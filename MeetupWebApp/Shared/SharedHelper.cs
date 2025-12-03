using MeetupWebApp.Features.Events.Shared;
using Microsoft.AspNetCore.Components;
using System.Web;


namespace MeetupWebApp.Shared
{
    public class SharedHelper
    {
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

    }
}
