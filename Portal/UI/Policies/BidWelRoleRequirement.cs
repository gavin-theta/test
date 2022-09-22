using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WelNetworks.BidWel.Portal.Policies
{
    public class BidWelRoleRequirement : AuthorizationHandler<BidWelRoleRequirement>, IAuthorizationRequirement
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BidWelRoleRequirement requirement)
        {
            var identity = context?.User?.Identities?.FirstOrDefault(i => !string.IsNullOrEmpty(i.Name));
            var authorized = false;
            try
            {
                var roles = identity?.Claims?.Where(c => c.Type == identity.RoleClaimType).Select(c => c.Value);

                var hasRole = roles?.Any(r => r.StartsWith("BidWel_")) ?? false;
                authorized = hasRole;
            }
            catch (Exception)
            {
                authorized = false;
            }

            if (authorized)
            {
                context?.Succeed(requirement);
            }
            else
            {
                context?.Fail();
            }


            return Task.CompletedTask;
        }
    }
}

