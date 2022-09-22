using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using WelNetworks.BidWel.Portal.Core.Contracts;
namespace WelNetworks.BidWel.Portal.Controllers
{

    public class ControllerBase : Controller
    {
        public ControllerBase(IDataAccess dataAccess, IConfiguration configuration, INotificationService notificationService)
        {
            this.dataAccess = dataAccess;
            this.configuration = configuration;
            this.notificationService = notificationService;
            culture = new CultureInfo("en-NZ");
        }


        internal readonly IDataAccess dataAccess;
        internal readonly IConfiguration configuration;
        internal readonly INotificationService notificationService;
        internal IFormatProvider culture;

    }
}
