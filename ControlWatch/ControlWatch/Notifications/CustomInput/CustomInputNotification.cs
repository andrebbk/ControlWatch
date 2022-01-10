using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToastNotifications.Core;
using ControlWatch.Notifications.CustomInput;
using ControlWatch.Utilities;

namespace ControlWatch.Notifications.CustomInput
{
    public class CustomInputNotification : NotificationBase, INotifyPropertyChanged
    {
        public CustomInputDisplayPart _displayPart;

        public CustomInputNotification(string message, string initialText, MessageOptions messageOptions) : base(message, messageOptions)
        {
            Message = message;
            InputText = initialText;
        }

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new CustomInputDisplayPart(this));

        #region binding properties

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

        private string _inputText;

        public string InputText
        {
            get
            {
                return _inputText;
            }
            set
            {
                _inputText = value;
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
