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

namespace ControlWatch.Notifications.CustomInput
{
    /// <summary>
    /// Interaction logic for CustomInputDisplayPart.xaml
    /// </summary>
    public partial class CustomInputDisplayPart : NotificationDisplayPart
    {
        public CustomInputDisplayPart(CustomInputNotification notification)
        {
            InitializeComponent();
            Bind(notification);
        }
    }
}
