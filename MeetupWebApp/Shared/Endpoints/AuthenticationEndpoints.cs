using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Web;

namespace MeetupWebApp.Shared.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static IEndpointRouteBuilder MapAuth(this IEndpointRouteBuilder app)
        {
            // Challenge the user to google authentication
            app.MapGet("/Authentication/{provider}", async (HttpContext ctx, string provider) =>
            {
                var returnUrl = ctx.Request.Query["returnurl"].ToString();

                var redirectUrl = "signin-callback";

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    redirectUrl = redirectUrl + $"?returnurl={returnUrl}";
                }

                var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };

                await ctx.ChallengeAsync(provider, properties);
            });

            // Checking the authentication and make sure it worked
            app.MapGet("/signin-callback", async (HttpContext ctx) =>
            {
                var returnUrl = ctx.Request.Query["returnurl"].ToString();

                var DecodedUrl = HttpUtility.UrlDecode(returnUrl);

                var result = await ctx.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!result.Succeeded || result.Principal is null || result.Principal.Claims is null)
                {
                    // Error Message Here
                    ctx.Response.Redirect("/");
                    return;
                }

                // No need to create a cookie becuase it was created by the OAuth Middleware

                if (!string.IsNullOrEmpty(DecodedUrl))
                {
                    ctx.Response.Redirect(DecodedUrl);
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
