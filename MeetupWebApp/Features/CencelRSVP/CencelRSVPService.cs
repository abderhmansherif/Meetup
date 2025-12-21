using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace MeetupWebApp.Features.CencelRSVP
{
    public class CencelRSVPService(IDbContextFactory<ApplicationDbContext> factory, IConfiguration configuration)
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = factory;
        public IConfiguration Configuration { get; } = configuration;

        public async Task<int> CencelRSVP(int eventId, int userId)
        {
            using var context = await Factory.CreateDbContextAsync();

            var rsvp = await context.RSVPs.Include(x => x.Event).Include(x => x.Transactions)
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

            if (rsvp == null)
            {
                return 0; // No RSVP found to cancel
            }

            rsvp.Status = SharedHelper.GetRSVPCancelStatus();

            if (rsvp.Event.Refundable)
            {
                StripeConfiguration.ApiKey = Configuration["Stripe:ApiKey"];
                var refund = await ProcessRefund(rsvp);

                if(refund is not null)
                {
                    var transaction = new Transaction
                    {
                        UserId = userId,
                        RASVPId = rsvp.Id,
                        RefundId = refund.Id,
                        RefundStatus = refund.Status,
                        CreatedAt = DateTime.Now
                    };
                }
            }

            await context.SaveChangesAsync();

            return rsvp.Id;
        }

        public async Task<Refund> ProcessRefund(RSVP rsvp)
        {
            var options = new RefundCreateOptions()
            {
                PaymentIntent = rsvp.Transactions.Where(x => !string.IsNullOrEmpty(x.PaymentId)).FirstOrDefault()?.PaymentId,
                Reason = "User_Requested",
            };

            var service = new RefundService();
            Refund refund = await service.CreateAsync(options);
            return refund;
        }


    }
}
