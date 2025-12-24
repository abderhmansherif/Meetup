using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.LeaveOrganizerReview
{
    public class LeaveOrganizerReviewService(IDbContextFactory<ApplicationDbContext> factory)
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = factory;

        public async Task<User?> GetOrganizerByIdAsync(int orgId)
        {
            await using var context = Factory.CreateDbContext();
            var organizer = await context.Users
                .FirstOrDefaultAsync(u => u.Id == orgId && u.UserRole == SharedHelper.GetOrganizerRole());

            return organizer;
        }
    }
}
