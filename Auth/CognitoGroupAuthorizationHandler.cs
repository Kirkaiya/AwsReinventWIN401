using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CognitoGroupAuthorizer
{
    class CognitoGroupAuthorizationHandler : AuthorizationHandler<CognitoGroupAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CognitoGroupAuthorizationRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "cognito:groups"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var group = context.User.FindFirst(c => c.Type == "cognito:groups").Value;

            if (group == requirement.CognitoGroup)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
