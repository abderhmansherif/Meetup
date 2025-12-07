using MeetupWebApp.Features.Events.Shared;
using Microsoft.AspNetCore.Components;
using System.Web;


namespace MeetupWebApp.Shared
{
    public class SharedHelper
    {
        public const string ATTENDEE_ROLE = "Attendee";
        public const string ORGANIZER_ROLE = "Organizer";
        public const string ADMINISTRATOR_ROLE = "Admin";

        public SharedHelper(NavigationManager NavigationManager)
        {
            this.NavigationManager = NavigationManager;
        }

        public NavigationManager NavigationManager { get; }

        public List<string> GetCategories ()
        {
            return Enum.GetNames(typeof(EventCategoriesEnum)).ToList();
        }


        public async Task<string> GetParamValueAsync (string QueryParam)
        {
            Uri Uri = new(NavigationManager.Uri);

            var Query = HttpUtility.ParseQueryString(Uri.Query);

            var QueryParamValue = Query[QueryParam] ?? string.Empty;

            return QueryParamValue;
        }

        public static string GetAttendeeRole() => ATTENDEE_ROLE;
        public static string GetOrganizerRole() => ORGANIZER_ROLE;
        public static string GetAdminRole() => ADMINISTRATOR_ROLE;



    }
}
