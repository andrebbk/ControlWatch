using ControlWatch.Commons.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlWatch.Windows.NewMovie
{
    /// <summary>
    /// Interaction logic for NewMovie_UserControl.xaml
    /// </summary>
    public partial class NewMovie_UserControl : System.Windows.Controls.UserControl
    {
        private MainWindow _mainWindow;

        private string LoadedMNovieCoverPath = "empty";

        public NewMovie_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;

            LoadNewMovie();
        }


        private void LoadNewMovie()
        {
            new Thread(() =>
            {
                var currentYear = DateTime.Now.Year;

                List<string> yearsList = new List<string>();

                for (int y = currentYear; y > 1979; y--)
                    yearsList.Add(y.ToString());

                if(yearsList != null && yearsList.Any())
                {
                    ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.ItemsSource = yearsList));

                    ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0)));
                }
                
            }).Start();            
        }


        private void ButtonLoadPic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Select movie cover";
            of.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";

            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadedMNovieCoverPath = of.FileName;
                TextBox_MovieCoverFileName.Text = of.FileName.Split('\\')[of.FileName.Split('\\').Count() - 1];

                MovieCover.Source = Utils.LoadImageToBitmapImageNoDecodeChange(of.FileName);
            }
            else
            {
                MovieCover.Source = null;
                LoadedMNovieCoverPath = "empty";
                TextBox_MovieCoverFileName.Clear();
            }
        }

        private void ButtonSaveMovie_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
