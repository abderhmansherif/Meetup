using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.ViewTransactions
{
    public class ViewTransactionsService(IDbContextFactory<ApplicationDbContext> factory)
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = factory;

        public async Task<List<Transaction>> GetTransactionsAsync(int orgId, DateTime startDate, DateTime endDate)
        {
            //Get the transactions that related to the organizer's events within the specified date range (attendees transactions for organizer's events)
            using var context = await Factory.CreateDbContextAsync();

            List<Transaction> transactions = await context.Transactions.AsNoTracking()
                .Include(x => x.RASVP)
                .ThenInclude(x => x.Event)
                .Include(x => x.User)
                .Where(x => x.RASVP!.Event!.OrganizerId == orgId && x.PaymentAt.Date >= startDate.Date && x.PaymentAt.Date <= endDate.Date)
                .OrderByDescending(x => x.PaymentAt)
                .ToListAsync();

            return transactions;
        }
    }
}
