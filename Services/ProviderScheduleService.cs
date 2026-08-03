using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Microsoft.EntityFrameworkCore;

namespace Family_and_Spa_Wellness.Services;

// Single source of truth for "is this provider actually working at this
// moment" - weekly recurring hours (ProviderShift) plus one-off day-off
// overrides (ProviderAvailability). Used everywhere a booking is created or
// re-validated (Book.razor's slot list, the final check at payment time, and
// the provider's own dashboard) so the rule can't drift between call sites.
public static class ProviderScheduleService
{
    public static async Task<ProviderSchedule> LoadAsync(AppDbContext db, List<int> providerIds, DateTime date)
    {
        if (providerIds.Count == 0)
        {
            return new ProviderSchedule(new List<ProviderShift>(), new HashSet<int>(), new HashSet<int>());
        }

        var dayOfWeek = date.DayOfWeek;
        var shiftsToday = await db.ProviderShifts
            .Where(s => providerIds.Contains(s.ProviderId) && s.DayOfWeek == dayOfWeek)
            .ToListAsync();

        var providersWithAnyShift = await db.ProviderShifts
            .Where(s => providerIds.Contains(s.ProviderId))
            .Select(s => s.ProviderId)
            .Distinct()
            .ToListAsync();

        var unavailableToday = await db.ProviderAvailability
            .Where(a => providerIds.Contains(a.ProviderId) && a.Date == date && !a.IsAvailable)
            .Select(a => a.ProviderId)
            .ToListAsync();

        return new ProviderSchedule(shiftsToday, providersWithAnyShift.ToHashSet(), unavailableToday.ToHashSet());
    }
}

public record ProviderSchedule(List<ProviderShift> ShiftsToday, HashSet<int> ProvidersWithAnyShift, HashSet<int> UnavailableToday)
{
    // A provider who has never touched their availability settings has no
    // ProviderShift rows at all - treat them as available every day (the
    // pre-existing default) rather than accidentally locking every
    // never-configured provider out of bookings entirely. Once a provider
    // has set up at least one shift, days with no shift row for them are
    // real days off.
    public bool IsWorking(int providerId, DateTime start, DateTime end)
    {
        if (UnavailableToday.Contains(providerId))
        {
            return false;
        }

        var todaysShifts = ShiftsToday.Where(s => s.ProviderId == providerId).ToList();
        if (todaysShifts.Count == 0)
        {
            return !ProvidersWithAnyShift.Contains(providerId);
        }

        return todaysShifts.Any(s => start.TimeOfDay >= s.StartTime && end.TimeOfDay <= s.EndTime);
    }
}
