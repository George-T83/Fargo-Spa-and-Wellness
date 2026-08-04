namespace Family_and_Spa_Wellness.Models;

// One row per (Date) exception to the regular weekly BusinessHours - a
// one-off early close or a full closure (holiday, etc). "No row" for a date
// means the regular weekly hours apply as-is.
public class BusinessHoursOverride
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsClosed { get; set; }

    // Only meaningful when IsClosed is false - the shop closes early at this
    // time instead of the regular CloseTime. Independent of OpenTime below;
    // a date can have either, both, or neither set.
    public TimeSpan? CloseTime { get; set; }

    // Only meaningful when IsClosed is false - the shop opens late at this
    // time instead of the regular OpenTime.
    public TimeSpan? OpenTime { get; set; }

    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
