using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.MakePayment
{
    public static class MakePaymentEndpoints
    {
        public static IEndpointRouteBuilder MapPayments(this IEndpointRouteBuilder app)
        {
            app.MapGet("/success-payment/{eventId:int}/{PaymentId}", 
                async (HttpContext ctx, MakePaymentService makePaymentService, int eventId, string PaymentId) =>
            {

                var factory = ctx.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

                // Gets the payment details
                var session = await makePaymentService.GetCheckoutSessionAsync(PaymentId);


                if (session is null)
                {
                    ctx.Response.Redirect("/payment-failed");
                    return;
                }

                // Create the RSVP record for user
                var userId = int.Parse(ctx.User.Claims
                    .FirstOrDefault(c => c.Type == SharedHelper.GetUserIdClaimType())!.Value);

                bool canESVP = await makePaymentService.CanRSVP(eventId, userId);

                if(!canESVP)
                {
                    ctx.Response.Redirect("/");
                    return;
                }

                var newRsvp = new RSVP
                {
                    EventId = eventId,
                    UserId = userId,
                    Status = SharedHelper.GetRSVPGoingStatus(),
                    RSVPDate = DateTime.UtcNow
                };

                using var context = await factory.CreateDbContextAsync();
                await context.RSVPs.AddAsync(newRsvp);
                await context.SaveChangesAsync();

                // Record the transaction
                var recordResult = await makePaymentService.RecordTransactionAsync(newRsvp.Id, userId, session);

                if(!recordResult)
                {
                    ctx.Response.Redirect("/");
                    return;
                }

                ctx.Response.Redirect($"/users/{userId}/manage-rsvp-events");
            });

            return app;
        }
    }
}
