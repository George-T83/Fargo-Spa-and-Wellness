using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Microsoft.EntityFrameworkCore;

namespace Family_and_Spa_Wellness.Services;

public static class ProviderShiftService
{
    public static async Task SaveWeeklyShifts(AppDbContext db, int providerId, IEnumerable<ProviderShift> rows)
    {
        var existing = await db.ProviderShifts
            .Where(s => s.ProviderId == providerId)
            .ToListAsync();

        foreach (var row in rows)
        {
            var match = existing.FirstOrDefault(e => e.DayOfWeek == row.DayOfWeek);
            if (match is null)
            {
                db.ProviderShifts.Add(new ProviderShift
                {
                    ProviderId = providerId,
                    DayOfWeek = row.DayOfWeek,
                    IsWorking = row.IsWorking,
                    StartTime = row.StartTime,
                    EndTime = row.EndTime,
                });
            }
            else
            {
                match.IsWorking = row.IsWorking;
                match.StartTime = row.StartTime;
                match.EndTime = row.EndTime;
            }
        }

        await db.SaveChangesAsync();
    }
}
