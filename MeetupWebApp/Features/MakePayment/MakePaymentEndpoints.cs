using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared;
using MeetupWebApp.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeetupWebApp.Features.MakePayment
{
    public static class MakePaymentEndpoints
    {
        public static IEndpointRouteBuilder MapPayments(this IEndpointRouteBuilder app)
        {
            app.MapGet("/success-payment/{eventId:int}/{PaymentId}", 
                async (HttpContext ctx, int eventId, string PaymentId) =>
            {
                var factory = ctx.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                var makePaymentService = ctx.RequestServices.GetRequiredService<MakePaymentService>();
                var RSVPEventService = ctx.RequestServices.GetRequiredService<RSVPEventService>();
                // Gets the payment details
                var session = await makePaymentService.GetCheckoutSessionAsync(PaymentId);

                if (session is null)
                {
                    ctx.Response.Redirect("/payment-failed");
                    return;
                }

                using var context = await factory.CreateDbContextAsync();


                // Create the RSVP record for user
                var userId = int.Parse(ctx.User.Claims
                    .FirstOrDefault(c => c.Type == SharedHelper.GetUserIdClaimType())!.Value);

                string Email = ctx.User.Claims
                   .FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;



                var ExistedRSVP = await context.RSVPs
                    .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

                if(ExistedRSVP is null)
                {
                    await RSVPEventService.AddRSVPEventAsync(eventId, Email, session.PaymentIntentId, session.PaymentStatus);
                }
                else
                {
                    ExistedRSVP.PaymentId = session.PaymentIntentId;
                    ExistedRSVP.Status = SharedHelper.GetRSVPGoingStatus();
                    ExistedRSVP.PaymentStatus = session.PaymentStatus;
                    ExistedRSVP.RefundId = string.Empty;
                    ExistedRSVP.RefundStatus = string.Empty;
                }

                await context.SaveChangesAsync();


                ctx.Response.Redirect($"/users/{userId}/manage-rsvp-events");

            }).RequireAuthorization();

            return app;
        }
    }
}
