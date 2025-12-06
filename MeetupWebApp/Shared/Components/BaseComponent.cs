using MeetupWebApp.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace MeetupWebApp.Shared.Components
{
    public class BaseComponent: ComponentBase
    {
        private bool ShouldClearContent = true;

        [Inject]
        LayoutService LayoutService { get; set; } = new ();
        public void ClearContent(bool decision = false) => ShouldClearContent = false;

        protected override async Task OnInitializedAsync()
        {
            if(ShouldClearContent)
            {
                LayoutService?.SetContentRender(null);
            }
        }

    }
}
