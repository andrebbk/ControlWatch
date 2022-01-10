using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications.Core;

namespace ControlWatch.Notifications.CustomMessage
{
    /// <summary>
    /// Interaction logic for CustomDisplayPart.xaml
    /// </summary>
    public partial class CustomDisplayPart : NotificationDisplayPart
    {
        public CustomDisplayPart(CustomNotification customNotification)
        {
            InitializeComponent();
            Bind(customNotification);
        }

        private void ButtonCloseNoty_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Notification.Close();
        }
    }
}
