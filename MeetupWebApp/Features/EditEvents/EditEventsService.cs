using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.Events.EditEvents
{
    public class EditEventsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IMapper mapper;

        public EditEventsService(IDbContextFactory<ApplicationDbContext> factory, IMapper mapper)
        {
            _factory = factory;
            this.mapper = mapper;
        }

        public async Task<EventViewModel> GetEventByIdAsync(int eventId)
        {
            using var context = await _factory.CreateDbContextAsync();
            var Event = await context.Events.FirstOrDefaultAsync(x => x.Id.Equals(eventId));

            return mapper.Map<EventViewModel>(Event);
        }

        public async Task UpdateEventAsync(EventViewModel model)
        {
            using var context = await _factory.CreateDbContextAsync();
            var Eventexist = await context.Events.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));

            if (Eventexist is null)
                throw new ArgumentException("there is no event with that id");

            mapper.Map(model, Eventexist);

            await context.SaveChangesAsync();
        }
    }
}
