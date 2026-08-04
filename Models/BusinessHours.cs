namespace Family_and_Spa_Wellness.Models;

// One row per day of the week (seeded for all seven). This is the shop's
// storefront window - the outer bound booking/reschedule slot generation and
// the Contact page hours both read from, separate from a specific provider's
// own ProviderShift hours which must fall inside it.
public class BusinessHours
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsOpen { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
}
