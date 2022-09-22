namespace Portal.Infrastructure
{
    public sealed class Enums
    {
        public enum NotificationType
        {
            INFO = 0,
            SUCCESS,
            WARNING,
            ERROR
        }

        public enum NotificationDisplay
        {
            ToastAndNotification = 0,
            ToastOnly = 2,
            NotificationOnly = 1
        }

    }
}
