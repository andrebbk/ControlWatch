using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications.Core;
using ControlWatch.Notifications.CustomMessage;

namespace ControlWatch.Notifications.CustomMessage
{
    public class CustomNotification : NotificationBase, INotifyPropertyChanged
    {
        public CustomDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new CustomDisplayPart(this));

        public CustomNotification(string title, string message, MessageOptions messageOptions) : base(message, messageOptions)
        {
            Title = title;
            Message = message;
        }

        #region binding properties
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _message;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public string Message
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
