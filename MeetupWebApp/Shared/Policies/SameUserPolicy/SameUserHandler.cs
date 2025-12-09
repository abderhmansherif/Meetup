using Microsoft.AspNetCore.Authorization;

namespace MeetupWebApp.Shared.Policies.SameUserPolicy
{
    public class SameUserHandler : AuthorizationHandler<SameUserRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SameUserHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SameUserRequirement requirement)
        {
            // 1) Get current authenticated user ID
            var authenticatedUserId = context.User?
                .FindFirst(SharedHelper.GetUserIdClaimType())?.Value;

            if (string.IsNullOrEmpty(authenticatedUserId))
                return Task.CompletedTask;

            // 2) Get userId from route
            var httpContext = _httpContextAccessor.HttpContext;
            var routeUserId = httpContext?.Request?.RouteValues["userId"]?.ToString();

            if (string.IsNullOrEmpty(routeUserId))
                return Task.CompletedTask;

            // 3) Compare
            if (routeUserId == authenticatedUserId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

}
