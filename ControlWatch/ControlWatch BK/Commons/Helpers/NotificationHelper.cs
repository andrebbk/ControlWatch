using ControlWatch.Notifications.CustomInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Events;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace ControlWatch.Commons.Helpers
{
    public static class NotificationHelper
    {
        public static readonly Notifier notifier;

        static NotificationHelper()
        {
            notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(3), MaximumNotificationCount.FromCount(5));
                cfg.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 10, 10);
                cfg.KeyboardEventHandler = new AllowedSourcesInputEventHandler(new[] { typeof(CustomInputDisplayPart) });
            });
        }
    }
}
