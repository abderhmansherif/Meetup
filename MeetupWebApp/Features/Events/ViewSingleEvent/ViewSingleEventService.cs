using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Features.Events.Shared;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.Events.ViewSingleEvent
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


    }
}
