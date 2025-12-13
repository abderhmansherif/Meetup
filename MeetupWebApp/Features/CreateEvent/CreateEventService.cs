using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.Events.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MeetupWebApp.Features.Events.CreateEvent
{
    public class CreateEventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IMapper _mapper;
        private EventViewModel? _eventViewModel;

        public CreateEventService(IDbContextFactory<ApplicationDbContext> factory, IMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }

        public async Task CraeteEventAsync(EventViewModel eventViewModel)
        {
            if (eventViewModel is null)
                throw new ArgumentNullException(nameof(eventViewModel));

            using (var context = await _factory.CreateDbContextAsync())
            {

                var Event = _mapper.Map<Event>(eventViewModel);

                context.Events?.Add(Event);

                await context.SaveChangesAsync();
            }
        }
       
    }
}
