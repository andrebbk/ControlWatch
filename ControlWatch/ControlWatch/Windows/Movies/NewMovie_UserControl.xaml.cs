using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Notifications.CustomMessage;
using ControlWatch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ControlWatch.Windows.Movies
{
    /// <summary>
    /// Interaction logic for NewMovie_UserControl.xaml
    /// </summary>
    public partial class NewMovie_UserControl : System.Windows.Controls.UserControl
    {
        private MainWindow _mainWindow;
        private MovieService movieService;

        private string LoadedMovieCoverPath = "empty";
        private int NewMovieYear = 0;

        public NewMovie_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            movieService = new MovieService();

            LoadNewMovie();
        }


        private void LoadNewMovie()
        {
            new Thread(() =>
            {
                var currentYear = DateTime.Now.Year;

                List<string> yearsList = new List<string>();

                for (int y = currentYear; y > Utils.GetAllowerYear(); y--)
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
                LoadedMovieCoverPath = of.FileName;
                TextBox_MovieCoverFileName.Text = of.FileName.Split('\\')[of.FileName.Split('\\').Count() - 1];

                MovieCover.Source = Utils.LoadImageToBitmapImageNoDecodeChange(of.FileName);
            }
            else
            {
                MovieCover.Source = null;
                LoadedMovieCoverPath = "empty";
                TextBox_MovieCoverFileName.Clear();
            }
        }

        private void ButtonSaveMovie_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateModel())
            {
                UtilsOperations.StartLoadingAnimation();

                int ratingValue = RatingMovie.Value.HasValue ? Convert.ToInt16((double)RatingMovie.Value * 10) : 0;

                var createMovieResult = movieService.CreateMovie(
                    TextBox_MovieTitle.Text.Trim(),
                    NewMovieYear,
                    CheckBoxIsFavorite.IsChecked.Value,
                    LoadedMovieCoverPath,
                    ratingValue,
                    TextBox_Observations.Text.Trim());

                if (createMovieResult == OutputTypeValues.Ok)
                {
                    ClearForm();
                    UtilsOperations.StopLoadingAnimation();
                    NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie saved successfully!");
                }
                else
                {
                    UtilsOperations.StopLoadingAnimation();
                    NotifyError(createMovieResult);
                }              
                                
            }

        }

        private bool ValidateModel()
        {
            bool isValid = true;

            if (String.IsNullOrWhiteSpace(TextBox_MovieTitle.Text))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie title is required!");
                isValid = false;
            }
            else if (String.IsNullOrWhiteSpace(TextBox_MovieCoverFileName.Text))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie cover is required!");
                isValid = false;
            }
            else if(!int.TryParse(ComboBoxYears.SelectedValue.ToString(), out NewMovieYear))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie year is invalid!");
                isValid = false;
            }
            else if(NewMovieYear < Utils.GetAllowerYear())
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie year is invalid!");
                isValid = false;
            }

            return isValid;
        }

        private void ClearForm()
        {
            //Title
            TextBox_MovieTitle.Clear();        
            
            //Year
            ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0);

            //IsFavorite
            CheckBoxIsFavorite.IsChecked = false;

            //Cover
            MovieCover.Source = null;
            TextBox_MovieCoverFileName.Clear();
            LoadedMovieCoverPath = "empty";

            //Rating 
            RatingMovie.Value = 0;

            //Observations
            TextBox_Observations.Clear();
        }

        private void NotifyError(OutputTypeValues result)
        {
            string msg = null;

            switch (result)
            {
                case OutputTypeValues.AlreadyExists:
                    msg = "Movie already exists!";
                    break;
                case OutputTypeValues.DataError:
                    msg = "Movie data is invalid!";
                    break;
                case OutputTypeValues.SavingCoverError:
                    msg = "An error has occurred saving cover!";
                    break;
                case OutputTypeValues.Error:
                default:
                    msg = "An error has occurred saving movie!";
                    break;
            }

            if(!String.IsNullOrEmpty(msg))
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", msg);
        }
    }
}
