using MeetupWebApp.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MeetupWebApp.Shared.Components
{
    public class BaseComponent: ComponentBase
    {
        private bool ShouldClearContent = true;

        [Inject]
        LayoutService LayoutService { get; set; } = new ();

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        public AuthenticationState? AuthenticationState;

        public bool IsAuthenticated = false;

        public void ClearContent(bool decision = false) => ShouldClearContent = false;

        protected override async Task OnInitializedAsync()
        {
            if(ShouldClearContent)
            {
                LayoutService?.SetContentRender(null);
            }

            AuthenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync ();

            IsAuthenticated = AuthenticationState.User?.Identity?.IsAuthenticated ?? false;
        }

        public void SaveAttendFooter() => ShouldClearContent = false;

        public bool IsAuthentdicated
        {
            get
            {
                return (AuthenticationState?.User?.Identity?.IsAuthenticated ?? false);
            }
        }

        public string Username
        {
            get
            {
                if(IsAuthentdicated)
                {
                    return (AuthenticationState?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty);
                }
                return string.Empty;
            }
        }

        public string Email
        {
            get
            {
                if (IsAuthentdicated)
                {
                    return (AuthenticationState?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty);
                }
                return string.Empty;
            }
        }

        public string UserId
        {
            get
            {
                if (IsAuthentdicated)
                {
                    return (AuthenticationState?.User?.Claims?.FirstOrDefault(x => x.Type == SharedHelper.GetUserIdClaimType())?.Value ?? string.Empty);
                }
                return string.Empty;
            }
        }

        public string UserRole
        {
            get
            {
                if (IsAuthentdicated)
                {
                    return (AuthenticationState?.User?.Claims?.FirstOrDefault(x => x.Type == SharedHelper.GetUserRoleClaimType())?.Value ?? string.Empty);
                }
                return string.Empty;
            }
        }

        public bool IsOrganizer
        {
            get
            {
                if(IsAuthentdicated && UserRole == SharedHelper.GetOrganizerRole())
                {
                    return true;
                }
                return false;
            }
        }

    }
}
