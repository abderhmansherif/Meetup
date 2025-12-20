using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.Events.Shared;
using MeetupWebApp.Shared;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Buffers.Text;

namespace MeetupWebApp.Features.MakePayment
{
    public class MakePaymentService(IDbContextFactory<ApplicationDbContext> Factory, IMapper mapper, IConfiguration configuration)  
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = Factory;
        public IMapper Mapper { get; } = mapper;
        public IConfiguration configuration { get; } = configuration;

        public async Task<EventViewModel> GetEventByIdAsync(int id)
        {
            using var context = await Factory.CreateDbContextAsync();

            var eventExist = await context.Events.FirstOrDefaultAsync(x => x.Id == id);

            return Mapper.Map<EventViewModel>(eventExist);
        }

        public async Task<string> CreateCheckoutSessionAsync(EventViewModel model, string baseUrl, string cencelUrl)
        {
            var options = new SessionCreateOptions()
            {
                LineItems = new()
                {
                    new SessionLineItemOptions()
                    {
                        PriceData = new()
                        {
                            ProductData = new()
                            {
                                Name = model.Title,
                                Description = $"Date & Time: {model.BeginDate:ddd MMM d} - {model.BeginTime:h:mm tt}",
                            },
                            Currency = "usd",
                            UnitAmount = (long)(model.TicketPrice.Value * 100)
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"{baseUrl.TrimEnd('/')}/success-payment/{model.Id}/{{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{baseUrl.TrimEnd('/')}/payment-failed"
            };

            StripeConfiguration.ApiKey = configuration["Stripe:ApiKey"];

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task<bool> CanRSVP(int eventId, int userId)
        {
            using var context = Factory.CreateDbContext();

            var EventExist = await context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var UserExist = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (EventExist == null)
            {
                return false;
            }

            if (UserExist == null)
            {
                return false;
            }

            var RSVPExist = await context.RSVPs.FirstOrDefaultAsync(x => x.UserId == UserExist.Id && x.EventId == EventExist.Id);

            if (RSVPExist is not null)
                return false;

            return true;
        }

        public async Task<Session> GetCheckoutSessionAsync(string paymentId)
        {
            if(string.IsNullOrEmpty(paymentId))
            {
                throw new ArgumentException("Payment ID is required", nameof(paymentId));
            }

            StripeConfiguration.ApiKey = configuration["Stripe:ApiKey"];
            var service = new SessionService();
            var session = await service.GetAsync(paymentId);
            return session;
        }

        public async Task<bool> RecordTransactionAsync(int rsvpId, int userId, Session session)
        {
            using var context = await Factory.CreateDbContextAsync();

            var rsvp = await context.RSVPs.FindAsync(rsvpId);
            if (rsvp == null)
            {
                return false; // RSVP not found
            }

            var transaction = new Data.Entities.Transaction
            {
                PaymentId = session.PaymentIntentId,
                PaymentStatus = session.PaymentStatus,
                UserId = userId,
                RASVPId = rsvp.Id,
                CreatedAt = DateTime.Now
            };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
            return true;
        }
    
    }


}
