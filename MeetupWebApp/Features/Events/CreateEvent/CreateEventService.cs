using MeetupBlazorWebApp.Features.Events.CreateEvent;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MeetupWebApp.Features.Events.CreateEvent
{
    public class CreateEventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        private EventViewModel? _eventViewModel;

        public CreateEventService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public void SetModel(EventViewModel model) 
                    => _eventViewModel = model ?? throw new ArgumentNullException(nameof(model));

        public DateTime GetBeginDateTime() 
                            => _eventViewModel.BeginDate.ToDateTime(_eventViewModel.BeginTime);
        public DateTime GetEndDateTime() 
                            => _eventViewModel.EndDate.ToDateTime(_eventViewModel.EndTime);

        public async Task CraeteEventAsync(EventViewModel eventViewModel)
        {
            if (eventViewModel is null)
                throw new ArgumentNullException(nameof(eventViewModel));

            using (var context = await _factory.CreateDbContextAsync())
            {
                context.Events?.Add(new Event()
                {
                    Title = eventViewModel.Title,
                    Description = eventViewModel.Description ?? string.Empty,
                    BeginDate = eventViewModel.BeginDate,
                    EndDate = eventViewModel.EndDate,
                    BeginTime = eventViewModel.BeginTime,
                    EndTime = eventViewModel.EndTime,
                    Capacity = eventViewModel.Capacity,
                    Category = eventViewModel.Category,
                    Location = eventViewModel.Location,
                    EventLink = eventViewModel.EventLink,
                });
                await context.SaveChangesAsync();
            }
        }
        public string ValidateDates()
        {
            if (_eventViewModel is null)
                 throw new ArgumentNullException("the event is null!!");

            var begin = GetBeginDateTime();
            var end = GetEndDateTime();

            if (begin > end)
                return "The Begin Date/Time should be earlier than the End Date/Time.";

            // checkin if the begin date is outdated
            if (begin < DateTime.Now)
                return "That Begin Date is outdated";

            // checkin if the End date is outdated
            if (end < DateTime.Now)
                return "That End Date is outdated";

            // checking the duration
            var duration = end - begin;
            if (duration > TimeSpan.FromDays(1))
                return "The Meetup duration cannot exceed 24 hours";

            // prevent setting dates to a past years
            var CurrentYear = DateTime.Now.Year;

            if (begin.Year < CurrentYear || end.Year < CurrentYear)
                return "Event dates nust not be set to past years";

            return string.Empty;
        }

        public string ValidateLocation()
        {
            if (_eventViewModel is null)
                throw new ArgumentNullException("the event is null!!");

            if (_eventViewModel.Category.Equals(EventCategoriesEnum.InPerson.ToString()) && string.IsNullOrWhiteSpace(_eventViewModel.Location))
                return "The Location is Required for In-Person Meetup";

            return string.Empty;
        }

        public string ValidateEventLink()
        {
            if (_eventViewModel is null)
                throw new ArgumentNullException("the event is null!!");

            if (_eventViewModel.Category.Equals(EventCategoriesEnum.Online) && string.IsNullOrWhiteSpace(_eventViewModel.EventLink))
                return "The Meetup Link is Required for Online Events";

            return string.Empty;
        }
    }
}
