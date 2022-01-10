using System;
using ToastNotifications;
using ToastNotifications.Core;

namespace ControlWatch.Notifications.CustomInput
{
    public static class CustomInputExtensions
    {
        public static void ShowCustomInput(this Notifier notifier, 
            string message,
            MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomInputNotification(message, message, messageOptions));
        }
    }
}
