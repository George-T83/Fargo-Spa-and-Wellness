namespace Family_and_Spa_Wellness.Models;

public class ServiceNote
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public User? Client { get; set; }

    public int AuthorId { get; set; }
    public User? Author { get; set; }

    // "Allergy", "Preference", or "General" - matches the bolt.new
    // Client Service Notes prototype's fixed category set.
    public string NoteType { get; set; } = "General";
    public string NoteText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
