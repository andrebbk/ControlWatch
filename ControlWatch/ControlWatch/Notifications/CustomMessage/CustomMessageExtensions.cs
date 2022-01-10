using ToastNotifications;
using ToastNotifications.Core;

namespace ControlWatch.Notifications.CustomMessage
{
    public static class CustomMessageExtensions
    {
        public static void ShowCustomMessage(this Notifier notifier, 
            string title, 
            string message,
            MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomNotification(title, message, messageOptions));
        }
    }
}
