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

namespace ControlWatch.Windows.TvShows
{
    /// <summary>
    /// Interaction logic for TvShowInfo_UserControl.xaml
    /// </summary>
    public partial class TvShowInfo_UserControl : UserControl
    {
        private MainWindow _mainWindow;

        public TvShowInfo_UserControl(MainWindow mainWindow, int? tvShowId)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }


        //Buttons
        private void ButtonGoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonLoadPic_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonSaveTvShow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonEditMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteTvShow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
