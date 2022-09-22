using Hangfire.Dashboard;
using System.Linq;
using System.Security.Claims;

namespace WelNetworks.BidWel.Portal
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow aadmin users to see the Dashboard!.

            var roles = httpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            var hasRole = roles.Any(r => r.StartsWith("BidWel_Administrator"));


            return true;// hasRole;
        }
    }
}
