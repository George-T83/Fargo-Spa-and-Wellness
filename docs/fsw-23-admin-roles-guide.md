# FSW-23: Admin User Role Management — implementation guide

This is guidance for closing out FSW-23, not a finished implementation. Written after
investigating why role-gated pages weren't working locally for anyone — the short
version: **there's no way to create an Admin account anywhere in the app yet, and
route-level authorization isn't wired up even where `[Authorize]` is used.** Both need
fixing as part of this ticket.

## The two gaps

### 1. No Admin (or Provider) account can exist

Every place a `User` gets created hardcodes `Role = "Client"`:

- `Components/Pages/Register.razor:176`
- `Data/AppDbContext.cs:43-45` (the 3 seeded demo users)
- `Migrations/20260728212759_SeedTestimonialsAndClients.cs:21-23`

There's no registration option, no seed data, and no promotion path that produces
anything else. Until this ticket adds one, the only way to get an Admin account
locally is a manual SQL update against your own dev DB:

```bash
sqlite3 fargospa.db "UPDATE Users SET Role='Admin' WHERE Email='you@example.com';"
```

That's a workaround for local testing, not a fix — FSW-23's acceptance criteria
("Role-assignment logic is implemented and enforces access control per role") implies
a real in-app mechanism for this.

### 2. `[Authorize]` attributes are currently inert — a critical prerequisite

`Components/Routes.razor` currently does this:

```razor
<Router AppAssembly="typeof(Program).Assembly" NotFoundPage="typeof(Pages.NotFound)">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
```

Plain `<RouteView>` does **not** check page-level `[Authorize]` attributes — that
requires `<AuthorizeRouteView>` instead. If you add `[Authorize(Roles = "Admin")]` to
a page right now, it will be silently ignored and the page will render for anyone,
authenticated or not. This needs to change first, before any admin page is built:

```razor
<Router AppAssembly="typeof(Program).Assembly" NotFoundPage="typeof(Pages.NotFound)">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)">
            <NotAuthorized>
                @if (context.User.Identity?.IsAuthenticated != true)
                {
                    <p>Please <a href="/login">sign in</a> to view this page.</p>
                }
                else
                {
                    <p>You don't have permission to view this page.</p>
                }
            </NotAuthorized>
        </AuthorizeRouteView>
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
```

`builder.Services.AddAuthorization()` is already registered in `Program.cs` from
FSW-11, so no new DI wiring is needed — just this Routes.razor change.

## Suggested implementation order

1. **Fix Routes.razor** as above. This alone doesn't add any admin feature, but it's
   the prerequisite everything else depends on. Worth its own small PR/commit.

2. **Bootstrap the first Admin account.** You can't create an Admin through an
   admin-only UI if no Admin exists yet to use it (chicken-and-egg). A common pattern:
   seed exactly one Admin account at startup if none exists, e.g. in `Program.cs`
   right after `db.Database.MigrateAsync()`:

   ```csharp
   if (!await db.Users.AnyAsync(u => u.Role == "Admin"))
   {
       var adminEmail = builder.Configuration["SEED_ADMIN_EMAIL"];
       var adminPassword = builder.Configuration["SEED_ADMIN_PASSWORD"];
       if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
       {
           var admin = new User { FirstName = "Admin", LastName = "User", Email = adminEmail, Phone = "000-000-0000", Role = "Admin" };
           admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, adminPassword);
           db.Users.Add(admin);
           await db.SaveChangesAsync();
       }
   }
   ```

   Read `SEED_ADMIN_EMAIL`/`SEED_ADMIN_PASSWORD` from `.env` (same pattern as the SMTP
   config — see `Program.cs`'s `LoadDotEnv`), so it's per-developer and never
   committed. This gives every dev a real Admin login on first run without a manual
   SQL step, and doesn't run again once an Admin already exists.

3. **Build the user management grid page** (`Components/Pages/Admin/Users.razor` or
   similar), gated with:

   ```razor
   @page "/admin/users"
   @attribute [Authorize(Roles = "Admin")]
   ```

   List all `Users` (id, name, email, current role), with a role dropdown per row
   (`Client` / `Provider` / `Admin`) and a save action that updates `User.Role` and
   calls `SaveChangesAsync()`. This is what satisfies "User management grid UI is
   built and lists all system users" in the acceptance criteria.

4. **Guard against self-lockout**: consider disallowing an Admin from demoting their
   own account (a simple check comparing the target `User.Id` against the current
   session's `ClaimTypes.NameIdentifier`), so nobody can accidentally lock themselves
   out of `/admin/users` with no other Admin to fix it.

5. **Verify enforcement end-to-end**: log in as a `Client`, confirm `/admin/users`
   shows the `NotAuthorized` content from step 1 instead of the page; log in as the
   seeded `Admin`, confirm the grid loads and a role change persists after a fresh
   login (re-authenticate to pick up the new role claim — the auth cookie won't
   reflect a role change until the next login, since roles are baked into the claims
   at sign-in time in `Services/AccountAuthEndpoints.cs`).

## Local setup note (for both of you)

Emailing doesn't require its own Brevo account — for local dev, Moses can use
George's existing verified sender. George should share the real `SMTP_PASSWORD`
value directly (Slack/1Password/etc.), never through chat or committed to git, for
Moses to paste into his own local `.env`. Everything else in `.env` (host, port,
login, from address) is the same for both of you since it's the shared team sender —
only the password is the actual secret.
