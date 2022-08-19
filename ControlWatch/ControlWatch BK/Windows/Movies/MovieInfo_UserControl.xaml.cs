using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Notifications.CustomMessage;
using ControlWatch.Popup;
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

        private int LoadedMovieId;
        private OperationTypeValues currentMode;
        private string LoadedMovieCoverPath = null;
        private int NewMovieYear = 0, NewMovieViews = 0;

        public MovieInfo_UserControl(MainWindow mainWindow, int? movieId)
        {
            InitializeComponent();

            _mainWindow = mainWindow;            
            moviesService = new MovieService();
            currentMode = OperationTypeValues.Info;

            if (movieId.HasValue && movieId.Value > 0)
                LoadMovieInfo(movieId.Value);
        }

        private void LoadMovieInfo(int movieId)
        {
            new Thread(() =>
            {
                if(currentMode == OperationTypeValues.Edit) ResetInfoMode();

                //load param
                LoadedMovieId = movieId;
                currentMode = OperationTypeValues.Info;
                LoadedMovieCoverPath = null;
                NewMovieYear = NewMovieViews = 0;

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

                    //Create date
                    LabelMovieAdded.Dispatcher.BeginInvoke((Action)(() => LabelMovieAdded.Content = movieInfo.CreateDate.ToString()));

                    //Rating movie
                    double ratingValue = (double)movieInfo.MovieRating / (double)10;
                    RatingMovie.Dispatcher.BeginInvoke((Action)(() => RatingMovie.Value = ratingValue));

                    //Observations
                    TextBox_Observations.Dispatcher.BeginInvoke((Action)(() => TextBox_Observations.Text = movieInfo.Observations));
                }

            }).Start();
        }

        private bool ValidateModel()
        {
            bool isValid = true;

            if (String.IsNullOrWhiteSpace(TextBox_MovieTitle.Text))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie title is required!");
                isValid = false;
            }
            else if (!int.TryParse(ComboBoxYears.SelectedValue.ToString(), out NewMovieYear))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie year is invalid!");
                isValid = false;
            }
            else if (NewMovieYear < 1980)
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie year is invalid!");
                isValid = false;
            }
            else if (!int.TryParse(ComboBoxViews.SelectedValue.ToString(), out NewMovieViews))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie views value is invalid!");
                isValid = false;
            }
            else if (NewMovieViews < 1)
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie views value is invalid!");
                isValid = false;
            }

            return isValid;
        }

        private void ResetInfoMode()
        {
            currentMode = OperationTypeValues.Info;
            TextBox_MovieTitle.Dispatcher.BeginInvoke((Action)(() => TextBox_MovieTitle.Text = "Edit Movie"));
            ButtonEditModeImage.Dispatcher.BeginInvoke((Action)(() => 
                ButtonEditModeImage.Source = Utils.LoadImageToBitmapFromResources("/ControlWatch;component/Resources/Buttons/Pencil.png"))
            );

            //Controls
            ButtonSaveMovie.Dispatcher.BeginInvoke((Action)(() => ButtonSaveMovie.Visibility = Visibility.Hidden));
            ButtonLoadPic.Dispatcher.BeginInvoke((Action)(() => ButtonLoadPic.Visibility = Visibility.Hidden));

            TextBox_MovieTitle.Dispatcher.BeginInvoke((Action)(() => TextBox_MovieTitle.IsEnabled = false));
            ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.IsEnabled = false));
            RatingMovie.Dispatcher.BeginInvoke((Action)(() => RatingMovie.IsEnabled = false));
            CheckBoxIsFavorite.Dispatcher.BeginInvoke((Action)(() => CheckBoxIsFavorite.IsEnabled = false));
            ComboBoxViews.Dispatcher.BeginInvoke((Action)(() => ComboBoxViews.IsEnabled = false));
            TextBox_Observations.Dispatcher.BeginInvoke((Action)(() => TextBox_Observations.IsReadOnly = true));
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
                case OutputTypeValues.MovieNotFound:
                    msg = "Movie not found!";
                    break;
                case OutputTypeValues.MovieCoverNotFound:
                    msg = "Movie cover not found!";
                    break;
                case OutputTypeValues.Error:
                default:
                    msg = "An error has occurred saving movie!";
                    break;
            }

            if (!String.IsNullOrEmpty(msg))
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", msg);
        }


        //Buttons
        private void ButtonGoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainWindow.SetMainContent(MenuOptionsTypeValues.Movies);
        }

        private void ButtonLoadPic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
            of.Title = "Select movie cover";
            of.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";

            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadedMovieCoverPath = of.FileName;
                TextBox_MovieCoverFileName.Text = of.FileName.Split('\\')[of.FileName.Split('\\').Count() - 1];

                MovieCover.Source = Utils.LoadImageToBitmapImageNoDecodeChange(of.FileName);
            }
            else
            {
                MovieCover.Source = null;
                LoadedMovieCoverPath = null;
                TextBox_MovieCoverFileName.Clear();
            }
        }

        private void ButtonSaveMovie_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateModel())
            {
                UtilsOperations.StartLoadingAnimation();

                int ratingValue = RatingMovie.Value.HasValue ? Convert.ToInt16((double)RatingMovie.Value * 10) : 0;

                var editMovieResult = moviesService.EditMovie(LoadedMovieId,
                    TextBox_MovieTitle.Text.Trim(),
                    NewMovieYear,
                    CheckBoxIsFavorite.IsChecked.Value,
                    LoadedMovieCoverPath,
                    ratingValue,
                    NewMovieViews,
                    TextBox_Observations.Text.Trim());

                if (editMovieResult == OutputTypeValues.Ok)
                {
                    LoadMovieInfo(LoadedMovieId);
                    UtilsOperations.StopLoadingAnimation();

                    NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movie edited successfully!");
                }
                else
                {
                    UtilsOperations.StopLoadingAnimation();
                    NotifyError(editMovieResult);
                }                    
            }
        }

        private void ButtonEditMode_Click(object sender, RoutedEventArgs e)
        {
            if(currentMode == OperationTypeValues.Info)
            {
                ButtonEditModeText.Text = "Info Movie";
                ButtonEditModeImage.Source = Utils.LoadImageToBitmapFromResources("/ControlWatch;component/Resources/Buttons/search-silver-icon.png");                
                currentMode = OperationTypeValues.Edit;

                //Controls
                ButtonSaveMovie.Visibility = Visibility.Visible;
                ButtonLoadPic.Visibility = Visibility.Visible;

                TextBox_MovieTitle.IsEnabled = true;
                ComboBoxYears.IsEnabled = true;
                RatingMovie.IsEnabled = true;
                CheckBoxIsFavorite.IsEnabled = true;
                ComboBoxViews.IsEnabled = true;
                TextBox_Observations.IsReadOnly = false;
            }
            else
            {
                ButtonEditModeText.Text = "Edit Movie";
                ButtonEditModeImage.Source = Utils.LoadImageToBitmapFromResources("/ControlWatch;component/Resources/Buttons/Pencil.png");
                currentMode = OperationTypeValues.Info;

                //Controls
                ButtonSaveMovie.Visibility = Visibility.Hidden;
                ButtonLoadPic.Visibility = Visibility.Hidden;

                TextBox_MovieTitle.IsEnabled = false;
                ComboBoxYears.IsEnabled = false;
                RatingMovie.IsEnabled = false;
                CheckBoxIsFavorite.IsEnabled = false;
                ComboBoxViews.IsEnabled = false;
                TextBox_Observations.IsReadOnly = true;
            }
        }

        private void ButtonDeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            string movieTitle = !String.IsNullOrEmpty(TextBox_MovieTitle.Text) ? TextBox_MovieTitle.Text : "this movie";
            ConfirmWindow popupConfirm = new ConfirmWindow("Are you sure you want to delete " + movieTitle  + "?");

            if (popupConfirm.ShowDialog() == true)
            {
                if (moviesService.DeleteMovieById(LoadedMovieId) == OutputTypeValues.Ok)
                {
                    NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Successfully deleted movie with id " + LoadedMovieId.ToString() + "!");
                    _mainWindow.SetMainContent(MenuOptionsTypeValues.Movies);
                }
                else
                    NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Error occurred trying delete movie with id " + LoadedMovieId.ToString() + "!");
            }
        }
    }
}
