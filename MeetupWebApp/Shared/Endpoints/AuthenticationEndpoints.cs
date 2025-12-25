using MeetupWebApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web;

namespace MeetupWebApp.Shared.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static IEndpointRouteBuilder MapAuth(this IEndpointRouteBuilder app)
        {
            // Challenge the user to google authentication for reqular Attendee
            app.MapGet("/Authentication/{provider}", async (HttpContext ctx, string provider) =>
            {
                // Read the url to redirect it later to the user
                var returnUrl = ctx.Request.Query["returnurl"].ToString();

                var redirectUrl = "signin-callback";

                var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };

                properties.Items["returnurl"] = returnUrl ?? string.Empty;

                await ctx.ChallengeAsync(provider, properties);
            });

            // Challenge the user to google authentication for Organizer
            app.MapGet("/Authentication/{provider}/BeOrganizer", async (HttpContext ctx, string provider) =>
            {
                var returnUrl = ctx.Request.Query["returnurl"].ToString();

                var redirectUrl = "signin-callback/BeOrganizer";

                var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };

                properties.Items["role"] = SharedHelper.GetOrganizerRole();

                properties.Items["returnurl"] = returnUrl ?? string.Empty;

                await ctx.ChallengeAsync(provider, properties);
            });

            // Call-Back Endpoint for Organizer
            app.MapGet("/signin-callback/BeOrganizer", async (HttpContext ctx) =>
            {
                var result = await ctx.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!result.Succeeded || result.Principal is null || result.Principal.Claims is null)
                {
                    // Error Message Here
                    ctx.Response.Redirect("/");
                    return;
                }

                var EncodedUrl = result.Properties.Items["returnurl"];

                var ReturnUrl = string.Empty;

                if (!string.IsNullOrEmpty(EncodedUrl))
                    ReturnUrl = HttpUtility.UrlDecode(EncodedUrl);


                // No need to create a cookie becuase it was created by the OAuth Middleware

                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    ctx.Response.Redirect(ReturnUrl);
                    return;
                }

                ctx.Response.Redirect("/");
                // TODO: return ErrorMessage
                return;
            });

            //If the User is already authenticated, this endpoint will elevate his role to Organizer
            app.MapGet("/BeOrganizer/{userId}", async (HttpContext ctx, string userId) =>
            {
                string returnUrl = ctx.Request.Query["returnurl"].ToString();

                var factory = ctx.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                var context = factory.CreateDbContext();

                var userExist = await context.Users.FirstOrDefaultAsync(x => x.Id == int.Parse(userId));

                if (userExist is null)
                {
                    ctx.Response.Redirect("/");
                    //Set Error Here
                    return;
                }
                userExist.UserRole = SharedHelper.GetOrganizerRole();
                await context.SaveChangesAsync();

                // Set the cookie 
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userExist.Username!),
                    new Claim(ClaimTypes.Email, userExist.Email!),
                    new Claim(SharedHelper.GetUserRoleClaimType(), userExist.UserRole!),
                    new Claim(SharedHelper.GetUserIdClaimType(), userExist.Id.ToString()!)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    ctx.Response.Redirect("/");
                    return;
                }

                ctx.Response.Redirect(returnUrl);
            })
                .RequireAuthorization("SameUser");

            // Checking the authentication and make sure it worked
            // Call-Back for regular Attendee
            app.MapGet("/signin-callback", async (HttpContext ctx) =>
            {
                var result = await ctx.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!result.Succeeded || result.Principal is null || result.Principal.Claims is null)
                {
                    // Error Message Here
                    ctx.Response.Redirect("/");
                    return;
                }

                var EncodedUrl = result.Properties.Items["returnurl"];

                var ReturnUrl = string.Empty;

                if (!string.IsNullOrEmpty(EncodedUrl))
                    ReturnUrl = HttpUtility.UrlDecode(EncodedUrl);

                // No need to create a cookie becuase it was created by the OAuth Middleware

                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    ctx.Response.Redirect(ReturnUrl);
                    return;
                }

                ctx.Response.Redirect("/");
                // TODO: return ErrorMessage
                return;
            });

            app.MapGet("/logout", async (HttpContext ctx) =>
            {
                var EncodedUrl = ctx.Request.Query["redirecturl"];
                var redirectUrl = string.Empty;

                if (!string.IsNullOrEmpty(EncodedUrl))
                    redirectUrl = HttpUtility.UrlDecode(EncodedUrl);


                if ((ctx.User?.Identity?.IsAuthenticated ?? false))
                {
                    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    if (!string.IsNullOrEmpty(redirectUrl))
                        ctx.Response.Redirect(redirectUrl);

                    return;
                }
                ctx.Response.Redirect("/");
                return;
            });

            return app;
        }
    }
}
