
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WelNetworks.BidWel.Portal.Core.Hubs
{
    public class NotificationHub : Hub
    {
        public NotificationHub(ILogger<NotificationHub> logger) : base()
        {
            this.logger = logger;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                await RemoveFromGroup();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"RemoveFromGroup on Disconnect failure:{Context?.ConnectionId}", ex);
            }

            await base.OnDisconnectedAsync(exception);
        }


        public override async Task OnConnectedAsync()
        {
            try
            {
                await AddToGroupAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"AddToGroup failure:{Context?.ConnectionId}", ex);
            }

            await base.OnConnectedAsync();
        }

        private async Task AddToGroupAsync()
        {
            logger.LogInformation($"Add to group:{Context?.ConnectionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, "heartbeat");
        }

        private async Task RemoveFromGroup()
        {
            logger.LogInformation($"Remove from group:{Context.ConnectionId}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "heartbeat");
        }

        private readonly ILogger<NotificationHub> logger;

    }
}
