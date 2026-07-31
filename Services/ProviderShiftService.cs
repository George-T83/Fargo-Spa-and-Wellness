using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Microsoft.EntityFrameworkCore;

namespace Family_and_Spa_Wellness.Services;

public static class ProviderShiftService
{
    // A provider can now have more than one shift block per day (e.g. a
    // morning block and an afternoon block with a lunch break between them),
    // so there's no longer a stable 1:1 row to update in place - the whole
    // week's rows are replaced with whatever the caller currently has.
    public static async Task SaveWeeklyShifts(AppDbContext db, int providerId, IEnumerable<ProviderShift> rows)
    {
        var existing = await db.ProviderShifts
            .Where(s => s.ProviderId == providerId)
            .ToListAsync();

        db.ProviderShifts.RemoveRange(existing);

        foreach (var row in rows)
        {
            db.ProviderShifts.Add(new ProviderShift
            {
                ProviderId = providerId,
                DayOfWeek = row.DayOfWeek,
                IsWorking = true,
                StartTime = row.StartTime,
                EndTime = row.EndTime,
            });
        }

        await db.SaveChangesAsync();
    }
}
