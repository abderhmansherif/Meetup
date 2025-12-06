using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.Events.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MeetupWebApp.Features.Events.ViewEvents
{
    public class ViewEventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IMapper _mapper;

        public ViewEventService(IDbContextFactory<ApplicationDbContext> factory, IMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            using (var context = await _factory.CreateDbContextAsync())
            {
                var Events = await (context.Events.AsNoTracking().ToListAsync() ?? Task.FromResult(new List<Event>()));

                var EventsViewModel = _mapper.Map<List<EventViewModel>>(Events);

                return EventsViewModel;
            }
        }
    }
}
