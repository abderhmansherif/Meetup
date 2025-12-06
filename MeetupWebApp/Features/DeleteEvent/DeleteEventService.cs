using MeetupWebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.DeleteEvent
{
    public class DeleteEventService
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; }

        public DeleteEventService(IDbContextFactory<ApplicationDbContext> factory)
        {
            Factory = factory;
        }

        public async Task<bool> IsDeletable(int id)
        {
            using var context = await Factory.CreateDbContextAsync();
            var meetup = await context.Events.FirstOrDefaultAsync(x => x.Id == id);

            if(meetup is not null)
            {
                if (meetup.BeginDate < DateOnly.FromDateTime(DateTime.Now)
                    || (meetup.BeginDate == DateOnly.FromDateTime(DateTime.Now)
                        && meetup.BeginTime <= TimeOnly.FromDateTime(DateTime.Now)))
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            using var context = await Factory.CreateDbContextAsync();

            var meetup = await context.Events.FirstOrDefaultAsync(x => x.Id == id);

            if(meetup is not null)
            {
                context.Remove(meetup);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
