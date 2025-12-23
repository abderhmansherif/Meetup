using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared.Services;
using MeetupWebApp.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MeetupWebApp.Features.Events.CreateEvent
{
    public class CreateEventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IMapper _mapper;
        private readonly RSVPEventService RSVPEventService;
        private EventViewModel? _eventViewModel;

        public CreateEventService(IDbContextFactory<ApplicationDbContext> factory, IMapper mapper, RSVPEventService RSVPEventService)
        {
            _factory = factory;
            _mapper = mapper;
            this.RSVPEventService = RSVPEventService;
        }

        public async Task CraeteEventAsync(EventViewModel eventViewModel)
        {
            if (eventViewModel is null)
                throw new ArgumentNullException(nameof(eventViewModel));

            using var context = await _factory.CreateDbContextAsync();
            
            // Create the Event
            var Event = _mapper.Map<Event>(eventViewModel);

            context.Events?.Add(Event);
            context.SaveChanges();

            //Create the RSVP for the Organizer
            await RSVPEventService.RSVPEventAsync(Event.Id, UserId: Event.OrganizerId);
        }
       
    }
}
