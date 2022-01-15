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

namespace ControlWatch.Popup
{
    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow(string msg)
        {
            InitializeComponent();

            if (!String.IsNullOrWhiteSpace(msg))
                TextBlockMsg.Text = msg;
        }

        //Buttons
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Back)
            {
                this.DialogResult = false;
                this.Close();
            }
            else if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
