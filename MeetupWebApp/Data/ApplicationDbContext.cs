using MeetupWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) :base(options)
        { }

        public DbSet<Event>? Events { get; set; }

    }
}
