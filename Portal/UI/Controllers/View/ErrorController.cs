using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WelNetworks.BidWel.Portal.Models;

namespace WelNetworks.BidWel.Portal.Controllers.View
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            logger.LogError("Error");
            SetViewBag("");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("500")]
        public IActionResult ApplicationError()
        {
            logger.LogError("Application");
            SetViewBag("Application");
            return View("500");
        }

        [HttpGet("401")]
        public IActionResult AuthenticationError()
        {
            logger.LogError("Authentication");
            SetViewBag("Authentication");
            return View("401");
        }

        [HttpGet("403")]
        public IActionResult ForbiddenError()
        {
            logger.LogError("Forbidden");
            SetViewBag("Forbidden");
            return View("403");
        }

        [HttpGet("404")]
        public IActionResult PageNotFound()
        {

            logger.LogError("Not Found");
            SetViewBag("Not Found");
            return View("404");
        }
        private void SetViewBag(string title)
        {
            ViewBag.Title = title;
            ViewBag.Nav = "Error";
        }

        private readonly ILogger<ErrorController> logger;
    }

}