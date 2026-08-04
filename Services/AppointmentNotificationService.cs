using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

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

public class AppointmentNotificationService(IEmailSender emailSender, NavigationManager navigationManager, IDbContextFactory<AppDbContext> dbContextFactory)
{
    private const string BusinessPhone = "(701) 555-0100";

    private Uri ProviderPortalUrl => new(new Uri(navigationManager.BaseUri), "/admin/dashboard");

    private Uri ContactUrl => new(new Uri(navigationManager.BaseUri), "/contact");

    // Every notification goes through here so a user's "email notifications"
    // preference (set on their profile) is always respected, regardless of
    // which flow (cancel, reschedule, admin override) triggered it.
    private async Task NotifyAsync(User recipient, string subject, string emailHtml)
    {
        if (recipient.NotifyByEmail)
        {
            await emailSender.SendAsync(recipient.Email, subject, emailHtml);
        }
    }

    public async Task NotifyCancelledAsync(User? client, User? provider, Service? service, DateTime start, DateTime end, ChangeActor actor, bool refunded = false)
    {
        var serviceName = service?.Name ?? "the treatment";
        var when = FormatWhen(start, end);
        var refundLine = refunded
            ? $"<p>Your payment of <strong>{service?.Price.ToString("C")}</strong> has been refunded and should appear on your original payment method within a few business days.</p>"
            : string.Empty;

        if (actor == ChangeActor.Client)
        {
            // The client initiated this themselves, but they still need a
            // receipt - especially when money actually moved. Without this,
            // a self-cancel refund left the client with no paper trail
            // beyond the transient on-page status message.
            if (client is not null)
            {
                await NotifyAsync(
                    client,
                    "Your appointment was cancelled - Fargo Spa and Wellness",
                    $"<p>Hi {client.FirstName},</p>" +
                    $"<p>Your <strong>{serviceName}</strong> appointment scheduled for {when} has been cancelled, as requested.</p>" +
                    refundLine +
                    ClientContactLine());
            }

            if (provider is not null)
            {
                await NotifyAsync(
                    provider,
                    "An appointment was cancelled - Fargo Spa and Wellness",
                    $"<p>Hi {provider.FirstName},</p>" +
                    $"<p>{ClientLine(client)} cancelled their <strong>{serviceName}</strong> appointment that was scheduled for {when}.</p>" +
                    (refunded ? $"<p>The client's payment of <strong>{service?.Price.ToString("C")}</strong> has been refunded.</p>" : string.Empty) +
                    PortalLink(ProviderPortalUrl));
            }
        }
        else if (actor == ChangeActor.Provider && client is not null)
        {
            await NotifyAsync(
                client,
                "Your appointment was cancelled - Fargo Spa and Wellness",
                $"<p>Hi {client.FirstName},</p>" +
                $"<p>Your <strong>{serviceName}</strong> appointment scheduled for {when} has been cancelled by {ProviderLine(provider)}. " +
                "Please contact us or book a new time that works for you.</p>" +
                refundLine +
                ClientContactLine());
        }
        else if (actor == ChangeActor.Admin)
        {
            // Admin-initiated: both sides are notified, but never with the
            // admin's own name/email - just a neutral "the spa" framing.
            if (client is not null)
            {
                await NotifyAsync(
                    client,
                    "Your appointment was cancelled - Fargo Spa and Wellness",
                    $"<p>Hi {client.FirstName},</p>" +
                    $"<p>Your <strong>{serviceName}</strong> appointment scheduled for {when} has been cancelled by our team. " +
                    "Please contact us or book a new time that works for you.</p>" +
                    refundLine +
                    ClientContactLine());
            }

            if (provider is not null)
            {
                await NotifyAsync(
                    provider,
                    "An appointment was cancelled - Fargo Spa and Wellness",
                    $"<p>Hi {provider.FirstName},</p>" +
                    $"<p>The <strong>{serviceName}</strong> appointment with {ClientLine(client)} scheduled for {when} has been cancelled by our team.</p>" +
                    (refunded ? $"<p>The client's payment of <strong>{service?.Price.ToString("C")}</strong> has been refunded.</p>" : string.Empty) +
                    PortalLink(ProviderPortalUrl));
            }
        }
    }

    public async Task NotifyRescheduledAsync(User? client, User? provider, Service? service, DateTime oldStart, DateTime oldEnd, DateTime newStart, DateTime newEnd, ChangeActor actor)
    {
        var serviceName = service?.Name ?? "the treatment";
        var oldWhen = FormatWhen(oldStart, oldEnd);
        var newWhen = FormatWhen(newStart, newEnd);
        var changeLine = $"moved from {oldWhen} to <strong>{newWhen}</strong>";

        if (actor == ChangeActor.Client)
        {
            // The client made the change, so they get their own confirmation
            // (not just the "someone else changed your appointment" framing
            // the provider gets) - both sides should always know a
            // reschedule happened, not just the one who didn't do it.
            if (client is not null)
            {
                await NotifyAsync(
                    client,
                    "Your appointment was rescheduled - Fargo Spa and Wellness",
                    $"<p>Hi {client.FirstName},</p>" +
                    $"<p>Your <strong>{serviceName}</strong> appointment has been rescheduled - it {changeLine}.</p>" +
                    ClientContactLine());
            }

            if (provider is not null)
            {
                await NotifyAsync(
                    provider,
                    "An appointment was rescheduled - Fargo Spa and Wellness",
                    $"<p>Hi {provider.FirstName},</p>" +
                    $"<p>{ClientLine(client)} rescheduled their <strong>{serviceName}</strong> appointment - it {changeLine}.</p>" +
                    PortalLink(ProviderPortalUrl));
            }
        }
        else if (actor == ChangeActor.Provider)
        {
            if (provider is not null)
            {
                await NotifyAsync(
                    provider,
                    "You rescheduled an appointment - Fargo Spa and Wellness",
                    $"<p>Hi {provider.FirstName},</p>" +
                    $"<p>You rescheduled the <strong>{serviceName}</strong> appointment with {ClientLine(client)} - it {changeLine}.</p>" +
                    PortalLink(ProviderPortalUrl));
            }

            if (client is not null)
            {
                await NotifyAsync(
                    client,
                    "Your appointment was rescheduled - Fargo Spa and Wellness",
                    $"<p>Hi {client.FirstName},</p>" +
                    $"<p>{ProviderLine(provider)} rescheduled your <strong>{serviceName}</strong> appointment - it {changeLine}.</p>" +
                    ClientContactLine());
            }
        }
        else if (actor == ChangeActor.Admin)
        {
            if (client is not null)
            {
                await NotifyAsync(
                    client,
                    "Your appointment was rescheduled - Fargo Spa and Wellness",
                    $"<p>Hi {client.FirstName},</p>" +
                    $"<p>Our team rescheduled your <strong>{serviceName}</strong> appointment - it {changeLine}.</p>" +
                    ClientContactLine());
            }

            if (provider is not null)
            {
                await NotifyAsync(
                    provider,
                    "An appointment was rescheduled - Fargo Spa and Wellness",
                    $"<p>Hi {provider.FirstName},</p>" +
                    $"<p>Our team rescheduled the <strong>{serviceName}</strong> appointment with {ClientLine(client)} - it {changeLine}.</p>" +
                    PortalLink(ProviderPortalUrl));
            }
        }
    }

    // Called any time money actually moves back to a client - a booking
    // conflict discovered at payment time, or a client cancelling outside
    // the 24-hour window. The provider and every Admin account get told,
    // since a refund is exactly the kind of event finance/ops needs
    // visibility into without having to notice it in the Stripe dashboard.
    public async Task NotifyRefundIssuedAsync(User? client, User? provider, Service? service, DateTime start, decimal amount, string reason)
    {
        var serviceName = service?.Name ?? "the treatment";
        var when = $"{start:dddd, MMMM d} at {start:h:mm tt}";
        var amountText = amount.ToString("C");

        await using var db = await dbContextFactory.CreateDbContextAsync();
        var admins = await db.Users.Where(u => u.Role == "Admin").ToListAsync();

        // Providers only get the abbreviated client name (ClientLine);
        // Admins are trusted with the full name, since they're the ones who
        // may need to look the client up.
        var providerBody = $"<p>A refund of <strong>{amountText}</strong> was issued for {ClientLine(client)}'s <strong>{serviceName}</strong> " +
                   $"appointment scheduled for {when}.</p>" +
                   $"<p>Reason: {reason}</p>" +
                   "<p>This amount has been excluded from revenue reporting.</p>";
        var adminBody = $"<p>A refund of <strong>{amountText}</strong> was issued for {(client is null ? "a client" : client.FullName)}'s <strong>{serviceName}</strong> " +
                   $"appointment scheduled for {when}.</p>" +
                   $"<p>Reason: {reason}</p>" +
                   "<p>This amount has been excluded from revenue reporting.</p>";

        if (provider is not null)
        {
            await NotifyAsync(provider, "Refund issued - Fargo Spa and Wellness", $"<p>Hi {provider.FirstName},</p>" + providerBody + PortalLink(ProviderPortalUrl));
        }

        foreach (var admin in admins)
        {
            await NotifyAsync(admin, "Refund issued - Fargo Spa and Wellness", $"<p>Hi {admin.FirstName},</p>" + adminBody);
        }
    }

    private static string PortalLink(Uri url) =>
        $"<p>View this here: <a href=\"{url}\">{url}</a></p>";

    private string ClientContactLine() =>
        $"<p>Questions? Call us at {BusinessPhone} or visit our <a href=\"{ContactUrl}\">Contact Us</a> page.</p>";

    // Providers only ever see a client's first name and last initial - never
    // the full name or email address. Full identity is reserved for Admins
    // (who see FullName directly in the admin UI) and the client themselves.
    private static string ClientLine(User? client) =>
        client is null ? "A client" : $"{client.FirstName} {LastInitial(client)}.";

    private static string LastInitial(User client) =>
        string.IsNullOrEmpty(client.LastName) ? "" : client.LastName[..1].ToUpperInvariant();

    private static string ProviderLine(User? provider) =>
        provider is null ? "your provider" : provider.FullName;

    private static string FormatWhen(DateTime start, DateTime end) =>
        $"{start:dddd, MMMM d} from {start:h:mm tt} to {end:h:mm tt}";
}
