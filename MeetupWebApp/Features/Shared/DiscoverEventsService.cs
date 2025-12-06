using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.Events.Shared
{
    public class DiscoverEventsService
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; }
        public IMapper Mapper { get; }

        public DiscoverEventsService(IDbContextFactory<ApplicationDbContext> Factory , IMapper mapper)
        {
            this.Factory = Factory;
            Mapper = mapper;
        }

        public async Task<List<EventViewModel>> GetEventsAsync(string searchWord)
        {
            using var context = await Factory.CreateDbContextAsync();

            var events = await SearchFilter(context, searchWord);

            if(events.Count == 0 && !string.IsNullOrEmpty(searchWord))
            {
                searchWord = null;
                events = await SearchFilter(context, searchWord);
            }

            return Mapper.Map<List<EventViewModel>>(events);
        }

        private async Task<List<Event>> SearchFilter (ApplicationDbContext context , string? searchWord)
        {

            return await context.Events.AsNoTracking().Where(x => (x.BeginDate > DateOnly.FromDateTime(DateTime.Now) 
                                                    || (x.BeginDate == DateOnly.FromDateTime(DateTime.Now) && 
                                                            x.BeginTime >= TimeOnly.FromDateTime(DateTime.Now))) && 
                                            (string.IsNullOrEmpty(searchWord) || x.Title.Contains(searchWord) || x.Description.Contains(searchWord) || x.Location.Contains(searchWord) || x.EventLink.Contains(searchWord)))
                                            .OrderByDescending(x => x.BeginDate)
                                            .ThenByDescending(x => x.BeginTime)
                                            .ToListAsync();
        }

    }
}
