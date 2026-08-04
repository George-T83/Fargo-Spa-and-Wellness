namespace Family_and_Spa_Wellness.Models;

public class ServiceCategory
{
    public const string DefaultIcon = "\U0001F31F";

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = DefaultIcon;
}
