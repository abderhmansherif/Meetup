using Microsoft.AspNetCore.Authorization;

namespace MeetupWebApp.Shared.Policies.OnlyOrganizersPolicy
{
    public class OnlyOrganizersRequirement : IAuthorizationRequirement
    {
    }
}
