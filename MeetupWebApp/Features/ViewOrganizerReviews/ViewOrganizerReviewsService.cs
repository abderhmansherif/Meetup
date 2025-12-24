using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.ViewOrganizerReviews
{
    public class ViewOrganizerReviewsService(IDbContextFactory<ApplicationDbContext> factory)
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = factory;

        public async Task<User?> GetOrganizerByIdAsync(int orgId)
        {
            await using var context = Factory.CreateDbContext();
            var organizer = await context.Users
                .FirstOrDefaultAsync(u => u.Id == orgId && u.UserRole == SharedHelper.GetOrganizerRole());
       
            return organizer;
        }
        
        public async Task<(int averageRating, int reviewCount)> GetOrganizerAverageRatingAsync(int orgId)
        {
            double averageRating = 0;
            int reviewCount = 0;

            using var context = await Factory.CreateDbContextAsync();

            reviewCount = await context.OrganizerReviews.CountAsync(x => x.OrganizerId == orgId);

            if(reviewCount > 0)
            {
                averageRating = await context.OrganizerReviews
                   .Where(r => r.OrganizerId == orgId)
                   .AverageAsync(r => r.Rating);
            }

            averageRating = (int)Math.Round(averageRating);

            return ((int)averageRating, reviewCount);
        }

        public async Task<List<OrganizerReview>?> GetOrganizerReviewsAsync(int orgId)
        {
            using var context = await Factory.CreateDbContextAsync();

            var reviews = await context.OrganizerReviews
                            .AsNoTracking().Include(x => x.ReviewerUser)
                            .Where(x => x.OrganizerId == orgId).OrderByDescending(x => x.ReviewDate)
                            .ToListAsync();

            return reviews;
        }
    }
}
