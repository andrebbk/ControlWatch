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

namespace ControlWatch.Windows.SideMenu
{
    /// <summary>
    /// Interaction logic for SideMenu_UserControl.xaml
    /// </summary>
    public partial class SideMenu_UserControl : UserControl
    {
        private MainWindow _mainWindow;

        public SideMenu_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
        }

        //Buttons
        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonMovies_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddMovie_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonTvShow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddTvShow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
