namespace Family_and_Spa_Wellness.Services;

// Sandbox/test-mode only (FSW-17) - values come from STRIPE_* env vars / .env,
// never committed. SecretKey and WebhookSecret must never reach the browser;
// only PublishableKey is safe to hand to client-side Stripe.js.
public class StripeSettings
{
    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}
