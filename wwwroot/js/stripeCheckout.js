let checkoutInstance = null;

function loadStripeScript() {
    return new Promise((resolve) => {
        if (window.Stripe) {
            resolve();
            return;
        }
        const script = document.createElement('script');
        script.src = 'https://js.stripe.com/v3/';
        script.onload = () => resolve();
        document.head.appendChild(script);
    });
}

export async function mountCheckout(publishableKey, clientSecret, containerId) {
    await loadStripeScript();
    const stripe = window.Stripe(publishableKey);
    checkoutInstance = await stripe.initEmbeddedCheckout({ clientSecret });
    checkoutInstance.mount('#' + containerId);
}

export function destroyCheckout() {
    if (checkoutInstance) {
        checkoutInstance.destroy();
        checkoutInstance = null;
    }
}
