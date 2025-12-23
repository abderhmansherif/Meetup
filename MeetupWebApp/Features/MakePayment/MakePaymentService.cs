using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared;
using MeetupWebApp.Shared.ViewModels;
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

        public async Task RecordTransactionAsync(int rsvpId, int userId, string paymentId, string paymentStatus)
        {
            // Implementation for recording transaction goes here
            using var context = await Factory.CreateDbContextAsync();

            var rsvpExist = await context.RSVPs.Include(x => x.Event).FirstOrDefaultAsync(x => x.Id == rsvpId);

            if (rsvpExist is not null && rsvpExist.Event is not null && rsvpExist.Event.HasCost && rsvpExist.Event.TicketPrice is not null)
            {
                var transaction = new Transaction()
                {
                    UserId = userId,
                    RASVPId = rsvpId,
                    Amount = rsvpExist.Event.TicketPrice.Value,
                    PaymentId = paymentId,
                    Status = paymentStatus,
                    PaymentType = SharedHelper.GetPaymentTypeCharge(),
                    PaymentAt = DateTime.Now
                };

                await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();
            }
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
    }
}
