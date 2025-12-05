using Microsoft.AspNetCore.Components;

namespace MeetupWebApp.Shared.Services
{
    public class LayoutService
    {
        public Action? OnViewEvent;
        public RenderFragment? renderFragment { get; set; }

        public void SetFooter(RenderFragment fragment)
        {
            renderFragment = fragment;
            OnViewEvent?.Invoke();
        }
    }
}
