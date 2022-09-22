using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Contracts;
using WelNetworks.BidWel.Portal.Models;

namespace WelNetworks.BidWel.Portal.Controllers.Views
{

    [Route("[controller]")]
    public class SearchController : ControllerBaseView
    {
        public SearchController(IDataAccess dataAccess, IConfiguration configuration, INotificationService notificationService, ILogger<SearchController> logger) : base(dataAccess, configuration, notificationService)
        {
            this.logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Search()
        {
            await SetViewBagItemsAsync();

            return View("Index");
        }

        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(SearchViewModel searchData)
        {
            await SetViewBagItemsAsync();
            var dates = searchData.DateRange.Split("-");
            var from = System.DateTime.Parse(dates[0], culture);
            var to = System.DateTime.Parse(dates[1], culture);

            var dispatches = await dataAccess.GetSearchAsync(from, to, searchData.Node, searchData.Product);

            var result = new SearchResultsModel { DateRange = searchData.DateRange, Node = searchData.Node, Product = searchData.Product, Results = dispatches, Total = dispatches.Count };

            return View("Index", result);
        }

        private readonly ILogger<SearchController> logger;
    }
}
