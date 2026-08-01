using Family_and_Spa_Wellness.Models;

namespace Family_and_Spa_Wellness.Services;

// Who actually made the change - drives who gets emailed and how the
// message is framed. The sender address is always the spa's own SMTP
// from-address (configured in SmtpOptions), never a staff member's
// personal email, so "actor" only affects recipients/wording, not
// anything about the admin's own identity leaking into the email.
public enum ChangeActor
{
    Client,
    Provider,
    Admin,
}

public class AppointmentNotificationService(IEmailSender emailSender)
{
    public async Task NotifyCancelledAsync(User? client, User? provider, Service? service, DateTime start, DateTime end, ChangeActor actor)
    {
        var serviceName = service?.Name ?? "the treatment";
        var when = FormatWhen(start, end);

        if (actor == ChangeActor.Client && provider is not null)
        {
            await emailSender.SendAsync(
                provider.Email,
                "An appointment was cancelled - Fargo Spa and Wellness",
                $"<p>Hi {provider.FirstName},</p>" +
                $"<p>{ClientLine(client)} cancelled their <strong>{serviceName}</strong> appointment that was scheduled for {when}.</p>");
        }
        else if (actor == ChangeActor.Provider && client is not null)
        {
            await emailSender.SendAsync(
                client.Email,
                "Your appointment was cancelled - Fargo Spa and Wellness",
                $"<p>Hi {client.FirstName},</p>" +
                $"<p>Your <strong>{serviceName}</strong> appointment scheduled for {when} has been cancelled by {ProviderLine(provider)}. " +
                "Please contact us or book a new time that works for you.</p>");
        }
        else if (actor == ChangeActor.Admin)
        {
            // Admin-initiated: both sides are notified, but never with the
            // admin's own name/email - just a neutral "the spa" framing.
            if (client is not null)
            {
                await emailSender.SendAsync(
                    client.Email,
                    "Your appointment was cancelled - Fargo Spa and Wellness",
                    $"<p>Hi {client.FirstName},</p>" +
                    $"<p>Your <strong>{serviceName}</strong> appointment scheduled for {when} has been cancelled by our team. " +
                    "Please contact us or book a new time that works for you.</p>");
            }

            if (provider is not null)
            {
                await emailSender.SendAsync(
                    provider.Email,
                    "An appointment was cancelled - Fargo Spa and Wellness",
                    $"<p>Hi {provider.FirstName},</p>" +
                    $"<p>The <strong>{serviceName}</strong> appointment with {ClientLine(client)} scheduled for {when} has been cancelled by our team.</p>");
            }
        }
    }

    public async Task NotifyRescheduledAsync(User? client, User? provider, Service? service, DateTime oldStart, DateTime oldEnd, DateTime newStart, DateTime newEnd, ChangeActor actor)
    {
        var serviceName = service?.Name ?? "the treatment";
        var oldWhen = FormatWhen(oldStart, oldEnd);
        var newWhen = FormatWhen(newStart, newEnd);
        var changeLine = $"moved from {oldWhen} to <strong>{newWhen}</strong>";

        if (actor == ChangeActor.Client && provider is not null)
        {
            await emailSender.SendAsync(
                provider.Email,
                "An appointment was rescheduled - Fargo Spa and Wellness",
                $"<p>Hi {provider.FirstName},</p>" +
                $"<p>{ClientLine(client)} rescheduled their <strong>{serviceName}</strong> appointment - it {changeLine}.</p>");
        }
        else if (actor == ChangeActor.Provider && client is not null)
        {
            await emailSender.SendAsync(
                client.Email,
                "Your appointment was rescheduled - Fargo Spa and Wellness",
                $"<p>Hi {client.FirstName},</p>" +
                $"<p>{ProviderLine(provider)} rescheduled your <strong>{serviceName}</strong> appointment - it {changeLine}.</p>");
        }
        else if (actor == ChangeActor.Admin)
        {
            if (client is not null)
            {
                await emailSender.SendAsync(
                    client.Email,
                    "Your appointment was rescheduled - Fargo Spa and Wellness",
                    $"<p>Hi {client.FirstName},</p>" +
                    $"<p>Our team rescheduled your <strong>{serviceName}</strong> appointment - it {changeLine}.</p>");
            }

            if (provider is not null)
            {
                await emailSender.SendAsync(
                    provider.Email,
                    "An appointment was rescheduled - Fargo Spa and Wellness",
                    $"<p>Hi {provider.FirstName},</p>" +
                    $"<p>Our team rescheduled the <strong>{serviceName}</strong> appointment with {ClientLine(client)} - it {changeLine}.</p>");
            }
        }
    }

    private static string ClientLine(User? client) =>
        client is null ? "A client" : $"{client.FullName} ({client.Email})";

    private static string ProviderLine(User? provider) =>
        provider is null ? "your provider" : provider.FullName;

    private static string FormatWhen(DateTime start, DateTime end) =>
        $"{start:dddd, MMMM d} from {start:h:mm tt} to {end:h:mm tt}";
}
