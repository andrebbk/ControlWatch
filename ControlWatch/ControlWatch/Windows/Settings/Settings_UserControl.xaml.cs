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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlWatch.Windows.Settings
{
    /// <summary>
    /// Interaction logic for Settings_UserControl.xaml
    /// </summary>
    public partial class Settings_UserControl : UserControl
    {
        private MainWindow _mainWindow;

        public Settings_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
    }
}
