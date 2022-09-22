using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Contracts;
using WelNetworks.BidWel.Portal.Core.Extensions;
using WelNetworks.BidWel.Portal.Core.Models;

namespace WelNetworks.BidWel.Portal.Core.Jobs
{
    public class HeartbeatCheck : IHeartbeatJob
    {
        public HeartbeatCheck(IDataAccess dataAccess, INotificationService notificationService, ILogger<Heartbeat> logger, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            this.notificationService = notificationService;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task ExecuteAsync(bool trigger)
        {
            try
            {
                var warnThreshold = configuration.GetSection("NotificationThreshold:Warn").Exists() ? int.Parse(configuration.GetSection("NotificationThreshold:Warn").Value) : 30;
                var errorThreshold = configuration.GetSection("NotificationThreshold:Error").Exists() ? int.Parse(configuration.GetSection("NotificationThreshold:Error").Value) : 60;
                var groupCode = configuration.GetSection("NotificationThreshold:GroupCode").Exists() ? configuration.GetSection("NotificationThreshold:GroupCode").Value : "IL";

                var heartbeat = await dataAccess.GetHeartbeatAsync(groupCode, warnThreshold, errorThreshold);//TODO: do we get all, or group for each product?
                var lastHeartbeat = heartbeat.LastRequestUTC.ToLocalDateTimeFromUtc();

                var heartbeatTitle = "Heartbeat";
               
                if(heartbeat.Status == Enums.NotificationType.SUCCESS) {
                   
                    logger.LogInformation($"{heartbeatTitle}. Last heartbeat was at {lastHeartbeat}");
                }
                else
                {
                    heartbeatTitle= "Heartbeat failure";
                    logger.LogWarning($"{heartbeatTitle}. Last heartbeat was at {lastHeartbeat}");
                }

                await notificationService.SendHeartbeatAsync($"{heartbeatTitle}", $"Last heartbeat was at {lastHeartbeat}", heartbeat, Enums.NotificationDisplay.ToastAndNotification);

                if (trigger)
                {
                    //workaround for 1 minute resolution for Hangfire, just queue anther task in 30s
                    BackgroundJob.Schedule<IHeartbeatJob>(j => j.ExecuteAsync(false), TimeSpan.FromSeconds(30));//first run
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Heartbeat Error:{DateTime.Now.ToLongTimeString()}", ex);
            }
        }

        internal readonly IDataAccess dataAccess;
        internal readonly INotificationService notificationService;
        internal readonly ILogger<Heartbeat> logger;
        internal readonly IConfiguration configuration;

    }
}