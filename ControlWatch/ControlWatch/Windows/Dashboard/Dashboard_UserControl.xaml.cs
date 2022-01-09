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

namespace ControlWatch.Windows.Dashboard
{
    /// <summary>
    /// Interaction logic for Dashboard_UserControl.xaml
    /// </summary>
    public partial class Dashboard_UserControl : UserControl
    {       
        private MainWindow _mainWindow;

        public Dashboard_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
        }
    }
}
