using MeetupWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) :base(options)
        { }

        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RSVP>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.RSVPs)
                   .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<RSVP>()
                   .HasOne(x => x.Event)
                   .WithMany(x => x.RSVPs)
                   .HasForeignKey(x => x.EventId);
        }

    }
}
