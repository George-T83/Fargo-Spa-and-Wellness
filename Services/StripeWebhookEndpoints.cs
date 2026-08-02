using System.Globalization;
using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Family_and_Spa_Wellness.Services;

// Sandbox/test-mode only (FSW-17). This is the source of truth for payment
// status - the browser's return-URL redirect is only a UI convenience and
// never itself marks an appointment paid. No Appointment row exists in the
// DB before this runs - the entire booking is carried in Stripe's session
// metadata (see Book.razor's HandleBooking), so an abandoned or failed
// checkout leaves nothing behind to clean up.
public static class StripeWebhookEndpoints
{
    public static void MapStripeWebhookEndpoints(this WebApplication app)
    {
        app.MapPost("/webhooks/stripe", async (
            HttpContext http,
            IDbContextFactory<AppDbContext> dbFactory,
            IOptions<StripeSettings> stripeSettings,
            IEmailSender emailSender,
            StripeCheckoutService stripeCheckout) =>
        {
            var json = await new StreamReader(http.Request.Body).ReadToEndAsync();
            var signature = http.Request.Headers["Stripe-Signature"];
            var webhookSecret = stripeSettings.Value.WebhookSecret;

            Event stripeEvent;
            try
            {
                stripeEvent = string.IsNullOrEmpty(webhookSecret)
                    ? EventUtility.ParseEvent(json)
                    : EventUtility.ConstructEvent(json, signature, webhookSecret);
            }
            catch (StripeException)
            {
                return Results.BadRequest();
            }

            if (stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
            {
                var baseUri = new Uri($"{http.Request.Scheme}://{http.Request.Host}");
                await MarkAppointmentPaidAsync(session, dbFactory, emailSender, stripeCheckout, baseUri);
            }

            return Results.Ok();
        });
    }

    public static async Task MarkAppointmentPaidAsync(Session session, IDbContextFactory<AppDbContext> dbFactory, IEmailSender emailSender, StripeCheckoutService stripeCheckout, Uri baseUri)
    {
        if (session.PaymentStatus != "paid")
        {
            return;
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        // Idempotent - a webhook retry or the browser return-url fallback
        // racing the webhook must not create the appointment twice.
        var alreadyProcessed = await db.Appointments.AnyAsync(a => a.StripeCheckoutSessionId == session.Id);
        if (alreadyProcessed)
        {
            return;
        }

        var metadata = session.Metadata;
        if (metadata is null
            || !int.TryParse(metadata.GetValueOrDefault("clientId"), out var clientId)
            || !int.TryParse(metadata.GetValueOrDefault("serviceId"), out var serviceId)
            || !DateTime.TryParse(metadata.GetValueOrDefault("startTime"), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var start)
            || !DateTime.TryParse(metadata.GetValueOrDefault("endTime"), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var end))
        {
            // Malformed or unrecognized session - nothing we can safely book.
            return;
        }

        var requestedProviderId = int.TryParse(metadata.GetValueOrDefault("providerId"), out var pid) && pid != 0 ? (int?)pid : null;

        var client = await db.Users.FindAsync(clientId);
        var service = await db.Services.FindAsync(serviceId);
        if (client is null || service is null)
        {
            return;
        }

        // Re-check the slot at the moment payment actually clears, since time
        // has passed since checkout started and someone else may have taken
        // it. If it's gone, refund the charge instead of double-booking.
        var overlapping = await db.Appointments
            .Where(a => a.AppointmentStatus != "Cancelled" && a.StartTime < end && start < a.EndTime && a.ProviderId != null)
            .Select(a => a.ProviderId)
            .ToListAsync();

        int? assignedProviderId;
        bool slotStillAvailable;
        if (requestedProviderId is not null)
        {
            slotStillAvailable = !overlapping.Contains(requestedProviderId);
            assignedProviderId = slotStillAvailable ? requestedProviderId : null;
        }
        else
        {
            var providers = await db.Users.Where(u => u.Role == "Provider").ToListAsync();
            assignedProviderId = providers.Count == 0
                ? null
                : providers.Select(p => (int?)p.Id).FirstOrDefault(id => !overlapping.Contains(id));
            slotStillAvailable = providers.Count == 0 || assignedProviderId is not null;
        }

        if (!slotStillAvailable)
        {
            await stripeCheckout.CreateRefundAsync(session.PaymentIntentId);
            await emailSender.SendAsync(
                client.Email,
                "Your appointment slot was taken - Fargo Spa and Wellness",
                $"<p>Hi {client.FirstName},</p>" +
                $"<p>We're sorry - the {start:dddd, MMMM d} at {start:h:mm tt} slot for <strong>{service.Name}</strong> was booked by someone else " +
                "while your payment was processing. You have not been charged; your payment has been fully refunded.</p>" +
                "<p>Please pick another time to rebook.</p>");
            return;
        }

        var appointment = new Appointment
        {
            ClientId = client.Id,
            ServiceId = service.Id,
            ProviderId = assignedProviderId,
            StartTime = start,
            EndTime = end,
            PaymentStatus = "Paid",
            StripeCheckoutSessionId = session.Id,
            StripePaymentIntentId = session.PaymentIntentId,
        };
        db.Appointments.Add(appointment);
        await db.SaveChangesAsync();

        var provider = assignedProviderId is null ? null : await db.Users.FindAsync(assignedProviderId.Value);

        // Every send here respects the recipient's own "email notifications"
        // preference from their profile - never sent unconditionally.
        if (client.NotifyByEmail)
        {
            await emailSender.SendAsync(
                client.Email,
                "Your payment receipt - Fargo Spa and Wellness",
                $"<p>Hi {client.FirstName},</p>" +
                $"<p>We've received your payment of <strong>{service.Price.ToString("C")}</strong> for " +
                $"<strong>{service.Name}</strong> on {start:dddd, MMMM d} at {start:h:mm tt}.</p>" +
                "<p>This was processed in Stripe test mode - no real charge was made.</p>" +
                "<p>Thank you, and we look forward to seeing you.</p>");
        }

        // The provider only hears about the booking once it's actually paid
        // for - see the comment in Book.razor's HandleBooking for why this
        // doesn't fire at booking-creation time instead.
        if (provider is not null && provider.NotifyByEmail)
        {
            var providerDashboardUrl = new Uri(baseUri, "/admin/dashboard");
            await emailSender.SendAsync(
                provider.Email,
                "New appointment assigned to you - Fargo Spa and Wellness",
                $"<p>Hi {provider.FirstName},</p>" +
                $"<p>You've been assigned a new appointment: <strong>{service.Name}</strong> " +
                $"with {ClientLine(client)} on {start:dddd, MMMM d} from {start:h:mm tt} to {end:h:mm tt}.</p>" +
                $"<p>View your schedule here: <a href=\"{providerDashboardUrl}\">{providerDashboardUrl}</a></p>");
        }
    }

    private static string ClientLine(Family_and_Spa_Wellness.Models.User? client) =>
        client is null ? "a client" : $"{client.FirstName} {client.LastName}";
}
