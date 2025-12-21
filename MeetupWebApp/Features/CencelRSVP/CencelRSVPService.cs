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

        public async Task<int> CencelRSVPAsync(int eventId, int userId)
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
                var refund = ProcessRefund(rsvp);

                if(refund is not null)
                {
                    // Update RSVP with refund details
                    rsvp.RefundId = refund.Id;
                    rsvp.RefundStatus = refund.Status;
                }
            }
            await context.SaveChangesAsync();

            return rsvp.Id;
        }

        public async Task<RSVP> GetRSVPByIdAsync(int rsvpId)
        {
            using var context = await Factory.CreateDbContextAsync();
            var rsvp = await context.RSVPs.FirstOrDefaultAsync(x => x.Id == rsvpId);
            return rsvp;
        }

        private  Refund ProcessRefund(RSVP rsvp)
        {
            var options = new RefundCreateOptions()
            {
                PaymentIntent = rsvp.PaymentId,
                Reason = "requested_by_customer",
            };

            var service = new RefundService();
            Refund refund =  service.Create(options);
            return refund;
        }


    }
}
