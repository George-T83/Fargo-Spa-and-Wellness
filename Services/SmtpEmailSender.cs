using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Family_and_Spa_Wellness.Services;

public class SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(_options.Login, _options.Password),
            EnableSsl = true,
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_options.Login, "Fargo Spa and Wellness"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };
        message.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Don't let a transactional email failure block the user-facing action (registration, booking, etc.)
            logger.LogError(ex, "Failed to send email to {ToEmail} via {Host}:{Port}", toEmail, _options.Host, _options.Port);
        }
    }
}
