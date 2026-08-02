using Family_and_Spa_Wellness.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Family_and_Spa_Wellness.Services;

// Sandbox/test-mode only (FSW-17). This is the source of truth for payment
// status - the browser's return-URL redirect is only a UI convenience and
// never itself marks an appointment paid.
public static class StripeWebhookEndpoints
{
    public static void MapStripeWebhookEndpoints(this WebApplication app)
    {
        app.MapPost("/webhooks/stripe", async (
            HttpContext http,
            IDbContextFactory<AppDbContext> dbFactory,
            IOptions<StripeSettings> stripeSettings,
            IEmailSender emailSender) =>
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
                await MarkAppointmentPaidAsync(session, dbFactory, emailSender);
            }

            return Results.Ok();
        });
    }

    public static async Task MarkAppointmentPaidAsync(Session session, IDbContextFactory<AppDbContext> dbFactory, IEmailSender emailSender)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var appointment = await db.Appointments
            .Include(a => a.Client)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.StripeCheckoutSessionId == session.Id);

        if (appointment is null || appointment.PaymentStatus == "Paid")
        {
            // Unknown session, or already processed by the webhook/return-url race - idempotent no-op.
            return;
        }

        appointment.PaymentStatus = "Paid";
        appointment.StripePaymentIntentId = session.PaymentIntentId;
        await db.SaveChangesAsync();

        if (appointment.Client is not null)
        {
            await emailSender.SendAsync(
                appointment.Client.Email,
                "Your payment receipt - Fargo Spa and Wellness",
                $"<p>Hi {appointment.Client.FirstName},</p>" +
                $"<p>We've received your payment of <strong>{appointment.Service?.Price.ToString("C")}</strong> for " +
                $"<strong>{appointment.Service?.Name}</strong> on {appointment.StartTime:dddd, MMMM d} at {appointment.StartTime:h:mm tt}.</p>" +
                "<p>This was processed in Stripe test mode - no real charge was made.</p>" +
                "<p>Thank you, and we look forward to seeing you.</p>");
        }
    }
}
