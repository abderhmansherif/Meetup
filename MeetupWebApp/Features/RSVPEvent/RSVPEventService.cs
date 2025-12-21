using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared;
using Microsoft.EntityFrameworkCore;
using System;

namespace MeetupWebApp.Features.RSVPEvent
{
    public class RSVPEventService
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; }

        public RSVPEventService(IDbContextFactory<ApplicationDbContext> factory)
        {
            Factory = factory;
        }

        public async Task<string> GetRSVPAttendeeStatusAsync(int eventId, string email)
        {
            using var context = Factory.CreateDbContext();

            var userExist = await context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if(userExist is null)
            {
                return string.Empty;
            }

            var RSVPExist = await context.RSVPs.FirstOrDefaultAsync(x => x.UserId == userExist.Id && x.EventId == eventId);

            if (RSVPExist is null)
            {
                return string.Empty;
            }

            return RSVPExist.Status ?? string.Empty;
        }

        public async Task<bool> UnAttnedRSVPAsync(int EventId, string Email)
        {
            using var context = Factory.CreateDbContext();

            var EventExist = await context.Events.FirstOrDefaultAsync(x => x.Id == EventId);
            var UserExist = await context.Users.FirstOrDefaultAsync(x => x.Email == Email);

            if (EventExist == null)
            {
                return false;
            }

            if (UserExist == null)
            {
                return false;
            }

            var RSVPExist = await context.RSVPs.FirstOrDefaultAsync(x => x.UserId == UserExist.Id && x.EventId == EventId);

            if (RSVPExist is null)
                return false;

            RSVPExist.Status = SharedHelper.GetRSVPNotGoingStatus();
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanRSVP(int eventId, string email)
        {
            using var context = Factory.CreateDbContext();

            var EventExist = await context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var UserExist = await context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (EventExist == null)
            {
                return false;
            }

            if (UserExist == null)
            {
                return false;
            }

            var RSVPExist = await context.RSVPs.FirstOrDefaultAsync(x => x.UserId == UserExist.Id && x.EventId == EventExist.Id);

            if (RSVPExist is not null && RSVPExist.Status == SharedHelper.GetRSVPGoingStatus())
                return false;

            return true;
        }

        public async Task<bool> AddRSVPEventAsync(int EventId, string Email)
        {
            if(!await CanRSVP(EventId, Email))
            {
                return false;
            }

            using var context = Factory.CreateDbContext();

            var UserExist = await context.Users.FirstOrDefaultAsync(x => x.Email == Email);

            var RSVPExist = await context.RSVPs.Include(x => x.Event).Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == Email && x.Event.Id == EventId);

            if (RSVPExist is null)
            {
                RSVPExist = new RSVP()
                {
                    UserId = UserExist.Id,
                    EventId = EventId,
                    RSVPDate = DateTime.Now,
                    Status = SharedHelper.GetRSVPGoingStatus()
                };

                await context.RSVPs.AddAsync(RSVPExist);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
