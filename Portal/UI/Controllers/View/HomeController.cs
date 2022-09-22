using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Contracts;

namespace WelNetworks.BidWel.Portal.Controllers.Views
{
    [Route("[controller]")]
    public class HomeController : ControllerBaseView
    {
        public HomeController(IDataAccess dataAccess, IConfiguration configuration, INotificationService notificationService, ILogger<HomeController> logger) : base(dataAccess, configuration, notificationService)
        {
            this.logger = logger;
        }

        [HttpGet("/signout/inactive")]
        public async Task Inactive()
        {
            await Signout("/signedout");
        }

        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            await SetViewBagItemsAsync();
            return View("Index");
        }

        [HttpGet("/signedout"), AllowAnonymous]
        public IActionResult SignedOut()
        {
            ViewBag.Title = "SignedOut";
            ViewBag.MaxInactiveTime = (configuration.GetValue("MaxInactiveTime", 59));

            return View("SignedOut");
        }

        [HttpGet("/signout")]
        public async Task SignOutUser()
        {
            await Signout("/signedout");
        }

        private readonly ILogger<HomeController> logger;
    }
}