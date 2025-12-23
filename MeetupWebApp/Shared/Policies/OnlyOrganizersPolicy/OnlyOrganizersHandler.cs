using Microsoft.AspNetCore.Authorization;

namespace MeetupWebApp.Shared.Policies.OnlyOrganizersPolicy
{
    public class OnlyOrganizersHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<OnlyOrganizersRequirement>
    {
        public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OnlyOrganizersRequirement requirement)
        {
            var HttpContext = HttpContextAccessor.HttpContext;

            if(HttpContext is null && HttpContext.User is null)
            {
                return Task.CompletedTask;
            }

            var userRole = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == SharedHelper.GetUserRoleClaimType())?.Value ?? string.Empty;

            if(string.IsNullOrEmpty(userRole))
            {
                return Task.CompletedTask;
            }

            if(userRole == SharedHelper.GetOrganizerRole())
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}
