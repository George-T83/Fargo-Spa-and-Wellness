namespace Family_and_Spa_Wellness.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "Client";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Whether this account receives email notifications about booking
    // changes (cancellations, reschedules, payment receipts). Applies to
    // every account type (Client, Provider, Admin).
    public bool NotifyByEmail { get; set; } = true;
}
