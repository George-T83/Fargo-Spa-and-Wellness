namespace Family_and_Spa_Wellness.Models;

public class ProviderShift
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsWorking { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? Provider { get; set; }
}
