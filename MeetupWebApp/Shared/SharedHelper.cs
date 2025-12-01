using MeetupBlazorWebApp.Features.Events.CreateEvent;

namespace MeetupWebApp.Shared
{
    public class SharedHelper
    {
        public List<string> GetCategories ()
        {
            return Enum.GetNames(typeof(EventCategoriesEnum)).ToList();
        }

        public async Task<bool> ShowMessageError(string msg)
        {
            if(!string.IsNullOrEmpty(msg))
            {

            }
            return false
        }
    }
}
