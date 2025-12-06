using Microsoft.AspNetCore.Components;

namespace MeetupWebApp.Shared.Services
{
    public class LayoutService
    {
        public Action? OnViewEvent;
        public RenderFragment? ContentRender { get; set; }

        public void SetContentRender(RenderFragment fragment)
        {
            ContentRender = fragment;
            OnViewEvent?.Invoke();
        }
    }
}
