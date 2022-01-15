using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlWatch.Windows.Movies
{
    /// <summary>
    /// Interaction logic for MovieInfo_UserControl.xaml
    /// </summary>
    public partial class MovieInfo_UserControl : UserControl
    {
        private MainWindow _mainWindow;
        private MovieService moviesService;

        public MovieInfo_UserControl(MainWindow mainWindow, int? movieId)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            moviesService = new MovieService();

            if(movieId.HasValue && movieId.Value > 0)
                LoadMovieInfo(movieId.Value);
        }

        private void LoadMovieInfo(int movieId)
        {
            new Thread(() =>
            {
                var movieInfo = moviesService.GetMovieById(movieId);
                if (movieInfo != null)
                {
                    //Movie title
                    TextBox_MovieTitle.Dispatcher.BeginInvoke((Action)(() => TextBox_MovieTitle.Text = movieInfo.MovieTitle));

                    //Movie year
                    var currentYear = DateTime.Now.Year;
                    List<string> yearsList = new List<string>();

                    for (int y = currentYear; y > 1979; y--)
                        yearsList.Add(y.ToString());

                    if (yearsList != null && yearsList.Any())
                    {
                        ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.ItemsSource = yearsList));

                        ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.SelectedItem = movieInfo.MovieYear.ToString()));
                    }

                    //Movie Favorite
                    if(movieInfo.IsFavorite)
                        CheckBoxIsFavorite.Dispatcher.BeginInvoke((Action)(() => CheckBoxIsFavorite.IsChecked = movieInfo.IsFavorite));

                    //Load movie cover
                    if (!String.IsNullOrEmpty(movieInfo.CoverPath))
                    {
                        TextBox_MovieCoverFileName.Dispatcher.BeginInvoke((Action)(() => TextBox_MovieCoverFileName.Text = movieInfo.CoverName));
                        MovieCover.Dispatcher.BeginInvoke((Action)(() => MovieCover.Source = Utils.LoadImageToBitmapStreamImage(movieInfo.CoverPath)));
                    }

                    //Movie views
                    List<string> viewsList = new List<string>();
                    for (int y = 0; y < 101; y++)
                        viewsList.Add(y.ToString());

                    if (viewsList != null && viewsList.Any())
                    {
                        ComboBoxViews.Dispatcher.BeginInvoke((Action)(() => ComboBoxViews.ItemsSource = viewsList));

                        ComboBoxViews.Dispatcher.BeginInvoke((Action)(() => ComboBoxViews.SelectedItem = movieInfo.NrViews.ToString()));
                    }

                    //CreateDate
                    LabelMovieAdded.Dispatcher.BeginInvoke((Action)(() => LabelMovieAdded.Content = movieInfo.CreateDate.ToString()));
                }

            }).Start();
        }

        //Buttons
        private void ButtonGoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainWindow.SetMainContent(MenuOptionsTypeValues.Movies);
        }

        private void ButtonLoadPic_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
