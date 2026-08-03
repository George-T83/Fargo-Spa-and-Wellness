using Family_and_Spa_Wellness.Components;
using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Family_and_Spa_Wellness.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

LoadDotEnv(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// Pin currency/date formatting to en-US regardless of the host OS locale -
// without this, a server with no locale configured (e.g. a bare Linux VM)
// falls back to the invariant culture, whose currency symbol is literally
// "¤" instead of "$".
var defaultCulture = new System.Globalization.CultureInfo("en-US");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, AdminBypassRolesHandler>();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.Configure<SmtpOptions>(options =>
{
    options.Host = builder.Configuration["SMTP_HOST"] ?? options.Host;
    if (int.TryParse(builder.Configuration["SMTP_PORT"], out var port))
    {
        options.Port = port;
    }
    options.Login = builder.Configuration["SMTP_LOGIN"] ?? options.Login;
    options.Password = builder.Configuration["SMTP_PASSWORD"] ?? options.Password;
    options.FromEmail = builder.Configuration["SMTP_FROM_EMAIL"] ?? options.FromEmail;
    options.FromName = builder.Configuration["SMTP_FROM_NAME"] ?? options.FromName;
});
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<AppointmentNotificationService>();

// Sandbox/test-mode only (FSW-17) - keys come from STRIPE_* env vars / .env, never committed.
builder.Services.Configure<StripeSettings>(options =>
{
    options.PublishableKey = builder.Configuration["STRIPE_PUBLISHABLE_KEY"] ?? options.PublishableKey;
    options.SecretKey = builder.Configuration["STRIPE_SECRET_KEY"] ?? options.SecretKey;
    options.WebhookSecret = builder.Configuration["STRIPE_WEBHOOK_SECRET"] ?? options.WebhookSecret;
});
builder.Services.AddScoped<StripeCheckoutService>();

// Render (and most PaaS hosts) terminate TLS at their edge and forward plain
// HTTP to the container, so without this the app thinks every request is
// insecure and UseHttpsRedirection() below loops forever. KnownNetworks/
// KnownProxies are cleared because the host's proxy isn't in the default
// trusted list - see https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    await db.Database.MigrateAsync();

    ServiceCategoryIcon.Configure(await db.ServiceCategories.ToListAsync());

    // Dev bootstrap: create exactly one Admin account from .env if none exists yet.
    if (!await db.Users.AnyAsync(u => u.Role == "Admin"))
    {
        var adminEmail = builder.Configuration["SEED_ADMIN_EMAIL"];
        var adminPassword = builder.Configuration["SEED_ADMIN_PASSWORD"];
        if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
        {
            var admin = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = adminEmail,
                Phone = "000-000-0000",
                Role = "Admin",
            };
            admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, adminPassword);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapAccountAuthEndpoints();
app.MapStripeWebhookEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void LoadDotEnv(string path)
{
    if (!File.Exists(path))
    {
        return;
    }

    foreach (var line in File.ReadAllLines(path))
    {
        var trimmed = line.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = trimmed.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = trimmed[..separatorIndex].Trim();
        var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"');

        // Don't clobber a value already set in the real process environment.
        if (Environment.GetEnvironmentVariable(key) is null)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
