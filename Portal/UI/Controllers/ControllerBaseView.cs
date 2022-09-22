using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Contracts;
namespace WelNetworks.BidWel.Portal.Controllers
{

    [Authorize(Policy = "BidWelRoleRequired")]
    public class ControllerBaseView : ControllerBase
    {

        public ControllerBaseView(IDataAccess dataAccess, IConfiguration configuration, INotificationService notificationService) : base(dataAccess, configuration, notificationService)
        {
            culture = new CultureInfo("en-NZ");
        }

        protected async Task SetViewBagItemsAsync(string navName = "")
        {
            var maxInactive = configuration.GetValue<int>("MaxInactiveTime", 55);
            var maxInactiveWarning = configuration.GetValue<int>("MaxInactiveWarningTime", 50);
            var claim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var notificationId = string.IsNullOrWhiteSpace(claim?.Value) ? User?.Identity?.Name : claim.Value;
            var appName = configuration.GetValue("ApplicationName", "BidWel Portal");
            var nds = await dataAccess.GetBlockAndNodeTypesAsync();
            var dis = await dataAccess.GetPrimaryDispatchTypesAsync();

            ViewBag.Nodes = nds.Select(a => new { Id = a, Name = a }).ToArray();
            ViewBag.DispatchTypes = dis.Select(a => new { Id = a, Name = a }).ToArray();
            ViewBag.UserName = User?.Identity?.Name;
            ViewBag.Nav = navName;
            ViewBag.MaxInactiveTime = maxInactive * 60000;
            ViewBag.MaxInactiveWarningTime = maxInactiveWarning * 60000;
            ViewBag.NotificationId = notificationId;
            ViewBag.AppName = appName;
            ViewBag.Title = $"{appName} {navName}";
        }

        protected async Task Signout(string redirectUri)
        {
            var prop = new AuthenticationProperties()
            {
                RedirectUri = redirectUri
            };
            // after signout this will redirect to your provided target          
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, prop);
        }

    }
}
