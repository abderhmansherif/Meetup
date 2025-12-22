using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Shared;
using MeetupWebApp.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.ManageUserRSVPEvents
{
    public class ManageUserRSVPEventsService(IDbContextFactory<ApplicationDbContext> factory, IMapper mapper)
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = factory;
        public IMapper Mapper { get; } = mapper;

        public async Task<List<EventViewModel>> GetUserRSVPAsync(int userId)
        {
            using var context = Factory.CreateDbContext();
            var events = await context.Events.Include(x => x.RSVPs)
                                       .Where(x => x.RSVPs.Any(x => x.UserId == userId && x.Status == SharedHelper.RSPV_GOING_STATUS))
                                       .ToListAsync();

            return Mapper.Map<List<EventViewModel>>(events);
        }

         
    }
}
