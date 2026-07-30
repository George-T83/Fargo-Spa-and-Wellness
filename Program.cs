using Family_and_Spa_Wellness.Components;
using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Services;
using Microsoft.EntityFrameworkCore;

LoadDotEnv(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

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
