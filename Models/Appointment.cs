namespace Family_and_Spa_Wellness.Models;

public class Appointment
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int ServiceId { get; set; }
    public int? ProviderId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // "Unpaid", "Paid", "Failed", or "Refunded" - set from Stripe Checkout
    // Session / webhook outcomes. Sandbox/test-mode only (FSW-17).
    public string PaymentStatus { get; set; } = "Unpaid";
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }

    public User? Client { get; set; }
    public Service? Service { get; set; }
    public User? Provider { get; set; }
}
