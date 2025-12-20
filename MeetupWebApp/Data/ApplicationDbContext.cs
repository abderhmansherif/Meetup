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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

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

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.Event)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.EventId)
                .IsRequired(true);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .IsRequired(false);

            modelBuilder.Entity<Event>()
                .HasOne(x => x.User)
                .WithOne(x => x.Event)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.User)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.RASVP)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.RASVPId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);


        }

    }
}
