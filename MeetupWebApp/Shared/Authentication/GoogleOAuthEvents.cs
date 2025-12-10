using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeetupWebApp.Shared.Authentication
{
    public static class GoogleOAuthEvents
    {
        public static async Task OnCreatingTicketEventAsync(OAuthCreatingTicketContext context)
        {
            if (context.Principal is null || context.Principal.Claims is null || context.Identity is null)
            {
                // Erorr Here
                context.HttpContext.Response.Redirect("/");
                return;
            }

            var UsernameClaim = context.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            var EmailClaim = context?.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            // DbContextFactory
            var Factory = context.HttpContext.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var contextDb = Factory.CreateDbContext();

            if (UsernameClaim is null && EmailClaim is null)
            {
                // Adding Error page
                return;
            }

            var userExist = await contextDb.Users.FirstOrDefaultAsync(x => x.Email == EmailClaim!.Value);

            // Set the Default role is Attendee
            var DesireRole = SharedHelper.GetAttendeeRole();

            // if there is an desire role in Authentication properties that passed by the Auth Endpoint
            // then we would change the desire role to that role 
            if(context.Properties.Items.TryGetValue("role", out string? role))
            {
                // Ensuring that the role is organizer not nothing else
                if (!string.IsNullOrEmpty(role) && role.Contains(SharedHelper.GetOrganizerRole()))
                {
                    DesireRole = role;
                }
            }

            // Hanlde If the user is actaully Organizer and just log in 
            //Ensure not be log in as a attendee

            if (userExist is null)
            {
                // that means user is not exists
                userExist = new User()
                {
                    Username = UsernameClaim!.Value,
                    Email = EmailClaim!.Value,
                    UserRole = DesireRole,
                };

                await contextDb.Users.AddAsync(userExist);
                await contextDb.SaveChangesAsync();
            }
            else
            {
                userExist.Username = UsernameClaim!.Value;

                // Ensure that if once the user is an organizer and set the user role with Attendee role 
                // to be still organizer
                if(!(userExist.UserRole == SharedHelper.GetOrganizerRole()))
                {
                    userExist.UserRole = DesireRole;
                }
                await contextDb.SaveChangesAsync();
            }

            // Adding UserID to calims to be in the cookie 
            context.Identity.AddClaim(new Claim(SharedHelper.GetUserIdClaimType(), userExist.Id.ToString()));
            context.Identity.AddClaim(new("UserRole", userExist.UserRole));

        }
    }
}
