namespace Family_and_Spa_Wellness.Models;

public class Testimonial
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public User? Client { get; set; }

    public int Rating { get; set; } // 1-5 stars
    public string ReviewText { get; set; } = string.Empty;

    // "Pending", "Approved", or "Rejected" — only "Approved" testimonials
    // are shown publicly (FSW-9's second acceptance criterion).
    public string ApprovalStatus { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}