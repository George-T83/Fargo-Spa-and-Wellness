namespace Family_and_Spa_Wellness.Models;

// One row per (ProviderId, Date) override. A provider with no row for a given
// date is available by default - rows only exist for dates the provider has
// explicitly toggled. Booking-flow code should treat "no row" as available
// and only exclude a date when a row exists with IsAvailable == false.
public class ProviderAvailability
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? Provider { get; set; }
}
