using MeetupWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MeetupWebApp.Features.Events.ViewEvents
{
    public class ViewEventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        public ViewEventService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<ICollection<EventViewModel>> GetEvents()
        {
            using (var context = await _factory.CreateDbContextAsync())
            {

            }
            return new List<EventViewModel>();
        }
    }
}
