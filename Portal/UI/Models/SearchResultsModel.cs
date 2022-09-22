using System.Collections.Generic;
using WelNetworks.BidWel.Portal.Core.Models;

namespace WelNetworks.BidWel.Portal.Models
{
    public class SearchResultsModel
    {
        public string Node { get; set; }

        public string DateRange { get; set; }

        public string Product { get; set; }

        public int Total { get; set; }

        public IList<DispatchInstruction> Results { get; set; }
    }
}
