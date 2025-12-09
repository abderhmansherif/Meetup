using Microsoft.AspNetCore.Authorization;

namespace MeetupWebApp.Shared.Policies.SameUserPolicy
{
    public class SameUserRequirement : IAuthorizationRequirement { }

}
