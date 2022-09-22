using System.Threading.Tasks;
using static WelNetworks.BidWel.Portal.Core.Enums;

namespace WelNetworks.BidWel.Portal.Core.Contracts
{
    public interface INotificationService
    {
        Task SendHeartbeatAsync(string title, string message, Models.Heartbeat heartbeat, NotificationDisplay display = NotificationDisplay.ToastAndNotification);

    }
}
