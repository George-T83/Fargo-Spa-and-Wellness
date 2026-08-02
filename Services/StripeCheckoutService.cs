using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Family_and_Spa_Wellness.Services;

// Sandbox/test-mode only (FSW-17). Embedded Checkout Sessions per Stripe's
// recommended API hierarchy for one-time, on-session web payments.
public class StripeCheckoutService(IOptions<StripeSettings> settings)
{
    // No Appointment row exists yet when the session is created - the booking
    // details travel entirely in Stripe's own metadata, and the Appointment
    // is only created once the webhook confirms payment actually succeeded
    // (see StripeWebhookEndpoints.MarkAppointmentPaidAsync). That way nothing
    // ever lands in the database for a checkout that's abandoned or fails.
    public async Task<Session> CreateEmbeddedSessionAsync(string serviceName, decimal amount, Uri returnUrl, Dictionary<string, string> bookingMetadata)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            UiMode = "embedded_page",
            ReturnUrl = $"{returnUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = serviceName,
                        },
                    },
                },
            ],
            Metadata = bookingMetadata,
        };

        var client = new SessionService(new Stripe.StripeClient(settings.Value.SecretKey));
        return await client.CreateAsync(options);
    }

    public async Task<Session> GetSessionAsync(string sessionId)
    {
        var client = new SessionService(new Stripe.StripeClient(settings.Value.SecretKey));
        return await client.GetAsync(sessionId);
    }

    public async Task<Refund> CreateRefundAsync(string paymentIntentId)
    {
        var client = new RefundService(new Stripe.StripeClient(settings.Value.SecretKey));
        return await client.CreateAsync(new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId,
        });
    }
}
