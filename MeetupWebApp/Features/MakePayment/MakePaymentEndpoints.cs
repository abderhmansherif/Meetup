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
                // Gets the Services
                var makePaymentService = ctx.RequestServices.GetRequiredService<MakePaymentService>();
                var RSVPEventService = ctx.RequestServices.GetRequiredService<RSVPEventService>();
                // Gets the payment details
                var session = await makePaymentService.GetCheckoutSessionAsync(PaymentId);

                if (session is null)
                {
                    ctx.Response.Redirect("/payment-failed");
                    return;
                }

                // Create the RSVP record for user, if the RSVP is already exist then update it with new status
                var userId = int.Parse(ctx.User.Claims
                    .FirstOrDefault(c => c.Type == SharedHelper.GetUserIdClaimType())!.Value);

                string Email = ctx.User.Claims
                   .FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;

                var rsvpId = await RSVPEventService.RSVPEventAsync(eventId, Email, paymentId: session.PaymentIntentId, paymentStatus: session.PaymentStatus);

                if(rsvpId <= 0)
                {
                    ctx.Response.Redirect($"/rsvp-error/{eventId}");
                    return;
                }

                //Record the transation
                await makePaymentService.RecordTransactionAsync(rsvpId, userId, session.PaymentIntentId, session.PaymentStatus);

                ctx.Response.Redirect($"/users/{userId}/manage-rsvp-events");

            }).RequireAuthorization();

            return app;
        }
    }
}
