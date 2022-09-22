using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Contracts;

namespace WelNetworks.BidWel.Portal.Controllers.Data
{
    [Route("Data/[controller]")]

    public class InstructionsController : ControllerBase
    {
        public InstructionsController(IDataAccess dataAccess, IConfiguration configuration, INotificationService notificationService, ILogger<InstructionsController> logger) : base(dataAccess, configuration, notificationService) { }

        [HttpGet("")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetDispatchInstructions(int pageNumber = 1, bool paged = true)
        {
            var (result, total) = await dataAccess.GetInstructionsAsync(pageNumber, paged);

            return Json(new { Results = result, Total = total });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetDispatchInstruction(int id)
        {
            var result = await dataAccess.GetInstructionAsync(id);
            return Json(result);
        }
    }
}
