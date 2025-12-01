using MeetupWebApp.Features.Events.Shared;

namespace MeetupWebApp.Shared
{
    public class SharedHelper
    {
        public List<string> GetCategories ()
        {
            return Enum.GetNames(typeof(EventCategoriesEnum)).ToList();
        }

    }
}
