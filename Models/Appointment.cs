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

    // "Scheduled", "Completed", "NoShow", or "Cancelled". Cancelling used to
    // hard-delete the row, which meant a cancelled appointment left no trace
    // for reporting - now it's a terminal status instead, so the reporting
    // pie chart (and any future audit trail) has something to count.
    public string AppointmentStatus { get; set; } = "Scheduled";

    // Whether this appointment has been moved at least once. Not mutually
    // exclusive with AppointmentStatus - a rescheduled appointment can still
    // end up Completed, NoShow, or Cancelled - so it's tracked separately
    // rather than as its own status bucket.
    public bool WasRescheduled { get; set; }

    public User? Client { get; set; }
    public Service? Service { get; set; }
    public User? Provider { get; set; }
}
