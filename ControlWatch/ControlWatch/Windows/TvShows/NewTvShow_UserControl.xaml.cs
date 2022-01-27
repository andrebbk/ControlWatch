using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Notifications.CustomMessage;
using ControlWatch.Services;
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

namespace ControlWatch.Windows.TvShows
{
    /// <summary>
    /// Interaction logic for NewTvShow_UserControl.xaml
    /// </summary>
    public partial class NewTvShow_UserControl : System.Windows.Controls.UserControl
    {
        private MainWindow _mainWindow;
        private TvShowService tvShowService;

        private string LoadedTvShowCoverPath = "empty";
        private int NewTvShowYear = 0;

        public NewTvShow_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            tvShowService = new TvShowService();

            LoadNewTvShow();
        }


        private void LoadNewTvShow()
        {
            new Thread(() =>
            {
                var currentYear = DateTime.Now.Year;

                List<string> yearsList = new List<string>();

                for (int y = currentYear; y > 1979; y--)
                    yearsList.Add(y.ToString());

                if (yearsList != null && yearsList.Any())
                {
                    ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.ItemsSource = yearsList));

                    ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0)));
                }

            }).Start();
        }

        private void ClearForm()
        {
            //Title
            TextBox_TvShowTitle.Clear();

            //Year
            ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0);

            //IsFavorite
            CheckBoxIsFavorite.IsChecked = false;

            //Cover
            TvShowCover.Source = null;
            TextBox_TvShowCoverFileName.Clear();
            LoadedTvShowCoverPath = "empty";

            //Rating 
            RatingTvShow.Value = 0;
        }

        private void NotifyError(OutputTypeValues result)
        {
            string msg = null;

            switch (result)
            {
                case OutputTypeValues.AlreadyExists:
                    msg = "TvShow already exists!";
                    break;
                case OutputTypeValues.DataError:
                    msg = "TvShow data is invalid!";
                    break;
                case OutputTypeValues.SavingCoverError:
                    msg = "An error has occurred saving cover!";
                    break;
                case OutputTypeValues.Error:
                default:
                    msg = "An error has occurred saving tvshow!";
                    break;
            }

            if (!String.IsNullOrEmpty(msg))
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", msg);
        }


        //Buttons
        private void ButtonLoadPic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Select movie cover";
            of.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";

            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadedTvShowCoverPath = of.FileName;
                TextBox_TvShowCoverFileName.Text = of.FileName.Split('\\')[of.FileName.Split('\\').Count() - 1];

                TvShowCover.Source = Utils.LoadImageToBitmapImageNoDecodeChange(of.FileName);
            }
            else
            {
                TvShowCover.Source = null;
                LoadedTvShowCoverPath = "empty";
                TextBox_TvShowCoverFileName.Clear();
            }
        }

        private void ButtonSaveTvShow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
