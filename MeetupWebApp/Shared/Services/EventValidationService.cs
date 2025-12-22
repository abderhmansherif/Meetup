using AutoMapper;
using MeetupWebApp.Shared.Enums;
using MeetupWebApp.Shared.ViewModels;

namespace MeetupWebApp.Shared.Services
{
    public class EventValidationService
    {
        private string _errorMessage { get; set; }

        public EventValidationService()
        {
            _errorMessage = string.Empty;
        }

        private DateTime GetBeginDateTime(EventViewModel model)
                           => model.BeginDate.ToDateTime(model.BeginTime);
        private DateTime GetEndDateTime(EventViewModel model)
                            => model.EndDate.ToDateTime(model.EndTime);

        public string ValidateEvent(EventViewModel model)
        {
            if(model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            _errorMessage = ValidateDates(model);
            if (!string.IsNullOrEmpty(_errorMessage))
                return _errorMessage;

            _errorMessage = ValidateLocation(model);
            if (!string.IsNullOrEmpty(_errorMessage))
                return _errorMessage;

            _errorMessage = ValidateEventLink(model);
            if (!string.IsNullOrEmpty(_errorMessage))
                return _errorMessage;

            return _errorMessage;
        }

        private string ValidateDates(EventViewModel model)
        {
            var begin = GetBeginDateTime(model);
            var end = GetEndDateTime(model);

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

        private string ValidateLocation(EventViewModel model)
        {
            if (model.Category.Equals(EventCategoriesEnum.InPerson.ToString()) && string.IsNullOrWhiteSpace(model.Location))
                return "The Location is Required for In-Person Meetup";

            return string.Empty;
        }

        private string ValidateEventLink(EventViewModel model)
        {
            if (model.Category.Equals(EventCategoriesEnum.Online.ToString()) && string.IsNullOrWhiteSpace(model.EventLink))
                return "The Meetup Link is Required for Online Events";

            return string.Empty;
        }

       
    }
}

