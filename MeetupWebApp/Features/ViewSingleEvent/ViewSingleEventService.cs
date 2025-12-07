using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.Events.Shared;
using MeetupWebApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.ViewSingleEvent
{
    public class ViewSingleEventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> Factory;

        private readonly IMapper Mapper;
        public ViewSingleEventService(IDbContextFactory<ApplicationDbContext> Factory, IMapper mapper)
        {
            this.Factory = Factory;
            Mapper = mapper;
        }

        public async Task<EventViewModel> GetEventByIdAsync(int eventId)
        {
            using (var context = await Factory.CreateDbContextAsync())
            {
                var Event = await context.Events.FirstOrDefaultAsync(x => x.Id.Equals(eventId));

                if (Event is not null)
                {
                    return Mapper.Map<EventViewModel>(Event);                                                                                                          
                }
                return new EventViewModel();    
            }
        }

        public async Task<List<User>> GetAttendees(int EventId)
        {
            using var context = Factory.CreateDbContext();
            var users = await context.RSVPs.AsNoTracking()
                                     .Include(x => x.User)
                                     .Where(x => x.EventId == EventId && x.Status == SharedHelper.GetRSVPGoingStatus())
                                     .Select(x => new User()
                                     {
                                         Id = x.UserId,
                                         Username = x.User!.Username,
                                         Email = x.User.Email,
                                     })
                                     .ToListAsync();
            
            return users;
        }


    }
}
