using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Viasoft.Licensing.LicensingManagement.Host.Extensions
{
    public static class AuthorizationRequirementExtensions
    {
        public static Task SucceedOrFailInCondition(this AuthorizationHandlerContext context, bool condition, IAuthorizationRequirement requirement)
        {
            if (condition)
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