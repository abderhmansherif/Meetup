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

                using var context = await factory.CreateDbContextAsync();


                // Create the RSVP record for user
                var userId = int.Parse(ctx.User.Claims
                    .FirstOrDefault(c => c.Type == SharedHelper.GetUserIdClaimType())!.Value);


                var ExistedRSVP = await context.RSVPs
                    .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

                if(ExistedRSVP is null)
                {
                    ExistedRSVP = new RSVP
                    {
                        EventId = eventId,
                        UserId = userId,
                        Status = SharedHelper.GetRSVPGoingStatus(),
                        RSVPDate = DateTime.UtcNow,
                        PaymentId = session.PaymentIntentId,
                        PaymentStatus = session.PaymentStatus,
                        RefundId = string.Empty, // 
                        RefundStatus = string.Empty,
                    };
                    await context.RSVPs.AddAsync(ExistedRSVP);

                }
                else
                {
                    ExistedRSVP.PaymentId = session.PaymentIntentId;
                    ExistedRSVP.Status = SharedHelper.GetRSVPGoingStatus();
                    ExistedRSVP.PaymentStatus = session.PaymentStatus;
                    ExistedRSVP.RefundId = string.Empty;
                    ExistedRSVP.RefundStatus = string.Empty;
                    ExistedRSVP.RSVPDate = DateTime.Now;
                }

                await context.SaveChangesAsync();


                ctx.Response.Redirect($"/users/{userId}/manage-rsvp-events");
            }).RequireAuthorization();

            return app;
        }
    }
}
