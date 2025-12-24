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

        public async Task<string> ReviewOrganaizerAsync(OrganizerReview review)
        {
            //Validation First
            if (review is null || review.OrganizerId < 1 ||
                review.ReviewUserId < 1 || review.Rating < 1 || review.Rating > 5 ||
                review.OrganizerId == review.ReviewUserId)
            {
                return "Invalid review, Please try again.";
            }
            using var context = Factory.CreateDbContext();

            //If there is dublicate review from same user for same organizer
            var isExists = await context.OrganizerReviews
                                    .AnyAsync(x => x.OrganizerId == review.OrganizerId 
                                                && x.ReviewUserId == review.ReviewUserId);

            if (isExists)
            {
                return "You have already reviewed this organizer.";
            }

            //checks if the reviewer user is already has an RSVP for that event
            var isExistRSVP = await context.RSVPs
                .Include(r => r.Event)
                .AnyAsync(r => r.UserId == review.ReviewUserId &&
                    r.Status == SharedHelper.GetRSVPGoingStatus() &&
                    r.Event.OrganizerId == review.OrganizerId);

            if(!isExistRSVP)
            {
                return "You must RSVP to the event before leaving a review and you can not review for past meetups.";
            }

            //Saving the review

            await context.OrganizerReviews.AddAsync(review);
            var saved = await context.SaveChangesAsync();

            if (saved > 0)
            {
                return string.Empty;
            }

            return "Failed to save the review.";
        }
    }
}
