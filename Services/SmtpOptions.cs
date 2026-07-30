namespace Family_and_Spa_Wellness.Services;

public class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
