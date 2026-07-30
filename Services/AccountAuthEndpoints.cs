using System.Security.Claims;
using Family_and_Spa_Wellness.Data;
using Family_and_Spa_Wellness.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Family_and_Spa_Wellness.Services;

public static class AccountAuthEndpoints
{
    public static void MapAccountAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/account/login", async (HttpContext http, IDbContextFactory<AppDbContext> dbFactory) =>
        {
            var form = await http.Request.ReadFormAsync();
            var email = form["email"].ToString().Trim();
            var password = form["password"].ToString();
            var returnUrl = form["returnUrl"].ToString();

            await using var db = await dbFactory.CreateDbContextAsync();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var hasher = new PasswordHasher<User>();
            var valid = user is not null &&
                hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;

            if (!valid)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Results.Redirect($"/login?error=1&returnUrl={System.Net.WebUtility.UrlEncode(returnUrl)}");
                }
                return Results.Redirect("/login?error=1");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await http.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            var redirectUrl = user.Role == "Admin" ? "/admin" : "/dashboard";
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                redirectUrl = returnUrl;
            }

            return Results.Redirect(redirectUrl);
        });

        app.MapPost("/account/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/");
        });
    }
}
