using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Microsoft.EntityFrameworkCore;

namespace Family_and_Spa_Wellness.Services;

// Single source of truth for "is the shop open, and until when, on this
// date" - the weekly BusinessHours row for that day of week, with any
// one-off BusinessHoursOverride applied on top. Used everywhere a booking
// window is generated or displayed (Book.razor, Reschedule.razor,
// Contact.razor) so the three can't drift out of sync the way the old
// hardcoded constants did.
public static class BusinessHoursService
{
    public static async Task<EffectiveHours> LoadForDateAsync(AppDbContext db, DateTime date)
    {
        var weekly = await db.BusinessHours.FirstOrDefaultAsync(h => h.DayOfWeek == date.DayOfWeek);
        var isOpen = weekly?.IsOpen ?? false;
        var openTime = weekly?.OpenTime ?? TimeSpan.Zero;
        var closeTime = weekly?.CloseTime ?? TimeSpan.Zero;

        var over = await db.BusinessHoursOverrides.FirstOrDefaultAsync(o => o.Date == date.Date);
        if (over is not null)
        {
            if (over.IsClosed)
            {
                isOpen = false;
            }
            else if (over.CloseTime is { } earlyClose && earlyClose < closeTime)
            {
                closeTime = earlyClose;
            }
        }

        return new EffectiveHours(isOpen, openTime, closeTime);
    }

    // Overrides falling within [today, today + daysAhead] - callers (e.g. the
    // Contact page) should keep daysAhead modest so the list stays a short,
    // scannable "heads up" rather than a dump of every exception on record.
    public static async Task<List<BusinessHoursOverride>> GetUpcomingOverridesAsync(AppDbContext db, DateTime today, int daysAhead)
    {
        var cutoff = today.Date.AddDays(daysAhead);
        return await db.BusinessHoursOverrides
            .Where(o => o.Date >= today.Date && o.Date <= cutoff)
            .OrderBy(o => o.Date)
            .ToListAsync();
    }
}

public record EffectiveHours(bool IsOpen, TimeSpan OpenTime, TimeSpan CloseTime);
