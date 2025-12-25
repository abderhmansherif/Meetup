using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.ViewOrganizerMeetups
{
    public class ViewOrganizerMeetupsService(IDbContextFactory<ApplicationDbContext> factory, IMapper mapper)
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = factory;
        public IMapper Mapper { get; } = mapper;

        public async Task<List<EventViewModel>> GetMeetupsAsync(int orgId)
        {
            using var context = await Factory.CreateDbContextAsync();

            var orgExists = await context.Users.AnyAsync(o => o.Id == orgId);

            if (!orgExists)
            {
                return new List<EventViewModel>();
            }

            var meetups = await context.Events.Where(x => x.OrganizerId == orgId)
                .OrderByDescending(x => x.BeginDate)
                .ThenByDescending(x => x.BeginTime)
                .ToListAsync();

            return Mapper.Map<List<EventViewModel>>(meetups);
        }
    }
}
