using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Contracts;
using WelNetworks.BidWel.Portal.Core.Hubs;
using static WelNetworks.BidWel.Portal.Core.Enums;

namespace WelNetworks.BidWel.Portal.Core.Services
{
    public class NotificationService : INotificationService
    {
        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                Formatting = Formatting.None
            };
        }

        public async Task SendHeartbeatAsync(string title, string message, Models.Heartbeat heartbeat, NotificationDisplay display = NotificationDisplay.ToastAndNotification)
        {
            var now = heartbeat.LastRequestUTC;
            var notification = JsonConvert.SerializeObject(new { Id = Guid.NewGuid(), Time = now, Title = title, Message = message, Priority = (int)heartbeat.Status, Display = (int)display }, jsonSerializerSettings);
            var group = hubContext.Clients.Group("heartbeat");

            await group.SendAsync("HEARTBEAT", notification).ConfigureAwait(true);


            logger.LogDebug("SendHeartbeatAsync:heartbeat:completed");
        }

        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly ILogger<NotificationService> logger;
    }
}
