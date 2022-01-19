using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Models.ViewModels;
using ControlWatch.Notifications.CustomMessage;
using ControlWatch.Popup;
using ControlWatch.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Movies_UserControl.xaml
    /// </summary>
    public partial class Movies_UserControl : UserControl
    {
        private MainWindow _mainWindow;
        private MovieService moviesService;

        private string searchTitle = null;
        private int? searchYear = null;
        private bool searchFavorite = false;
        private int? searchRating = null;

        //Pagination
        private int pagNumber = 1;
        private int pagLastNumber = 1;
        private int IPP = 200;

        public Movies_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;

            this.moviesService = new MovieService();

            LoadMovieList();
        }

        private void LoadMovieList()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var moviesList = moviesService.GetMovies(((pagNumber - 1) * IPP), IPP, searchTitle, searchYear, searchFavorite, searchRating);              

                if (moviesList != null && moviesList.Any())
                {
                    int? nMovies = moviesService.GetMoviesCount(searchTitle, searchYear, searchFavorite, searchRating);
                    LoadPaginationForPage(nMovies.HasValue ? nMovies.Value : moviesList.Count());

                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = null));
                    ObservableCollection<MoviesViewModel> moviesToShow = new ObservableCollection<MoviesViewModel>();

                    foreach (var item in moviesList)
                    {
                        //load imagem
                        if(!String.IsNullOrEmpty(item.MovieCoverPath))
                            item.MovieCover = Utils.LoadImageToBitmapStreamImage(item.MovieCoverPath);

                        moviesToShow.Add(item);
                    }                    

                    //BINDING
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = moviesToShow));

                    //RESTART SCROLLBAR
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ScrollIntoView(ListViewMovies.Items[0])));                    
                }

                //load filter
                LoadMoviesFilter();

                UtilsOperations.StopLoadingAnimation();

            }).Start();
        }

        private void LoadMoviesFilter()
        {            
            //Movie year filter
            List<string> yearsList = new List<string>();
            yearsList.Add("");

            var currentYear = DateTime.Now.Year;
            for (int y = currentYear; y > 1979; y--)
                yearsList.Add(y.ToString());

            if (yearsList != null && yearsList.Any())
            {
                ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.ItemsSource = yearsList));

                ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0)));
            }

            //Movie rating filter
            List<string> ratingList = new List<string>();
            ratingList.Add("");

            for (int y = 1; y < 11; y++)
                ratingList.Add(y.ToString());

            if (ratingList != null && ratingList.Any())
            {
                ComboBoxRatings.Dispatcher.BeginInvoke((Action)(() => ComboBoxRatings.ItemsSource = ratingList));

                ComboBoxRatings.Dispatcher.BeginInvoke((Action)(() => ComboBoxRatings.SelectedItem = ComboBoxRatings.Items.GetItemAt(0)));
            }
        }

        private void ReloadMoviesList()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var moviesList = moviesService.GetMovies(((pagNumber - 1) * IPP), IPP, searchTitle, searchYear, searchFavorite, searchRating);

                if (moviesList != null)
                {
                    int? nMovies = moviesService.GetMoviesCount(searchTitle, searchYear, searchFavorite, searchRating);
                    LoadPaginationForPage(nMovies.HasValue ? nMovies.Value : moviesList.Count());

                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = null));
                    ObservableCollection<MoviesViewModel> moviesToShow = new ObservableCollection<MoviesViewModel>();

                    foreach (var item in moviesList)
                    {
                        //load imagem
                        if (!String.IsNullOrEmpty(item.MovieCoverPath))
                            item.MovieCover = Utils.LoadImageToBitmapStreamImage(item.MovieCoverPath);

                        moviesToShow.Add(item);
                    }

                    //BINDING
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = moviesToShow));

                    //RESTART SCROLLBAR
                    if(moviesToShow.Any())
                        ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ScrollIntoView(ListViewMovies.Items[0])));
                }

                UtilsOperations.StopLoadingAnimation();
            }).Start();
        }

        private void SearchMoviesList()
        {
            //Load filters
            searchTitle = TextBoxSearchTerm.Text;
            searchYear = null;
            searchFavorite = CheckBoxIsFavorite.IsChecked.Value;
            searchRating = null;

            if (ComboBoxYears.SelectedValue != null)
            {
                int outYear = 0;
                if (int.TryParse(ComboBoxYears.SelectedValue.ToString(), out outYear))
                {
                    searchYear = outYear;
                }
            }

            if (ComboBoxRatings.SelectedValue != null)
            {
                int outRating = 0;
                if (int.TryParse(ComboBoxRatings.SelectedValue.ToString(), out outRating))
                {
                    searchRating = outRating;
                }
            }

            if (!String.IsNullOrEmpty(searchTitle) || searchYear != null || searchFavorite || searchRating != null)
            {
                ReloadMoviesList();
            }
        }

        //Event Actions
        private void ListViewMovies_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewMovies.SelectedItem != null)
            {
                _mainWindow.SetMainContent(MenuOptionsTypeValues.MovieInfo, ((MoviesViewModel)ListViewMovies.SelectedItem).MovieId);
            }
        }

        private void OpenMovie_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewMovies.SelectedItem != null)
            {
                _mainWindow.SetMainContent(MenuOptionsTypeValues.MovieInfo, ((MoviesViewModel)ListViewMovies.SelectedItem).MovieId);
            }
        }

        private void AddView_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewMovies.SelectedItem != null)
            {
                int movieId = ((MoviesViewModel)ListViewMovies.SelectedItem).MovieId;
                string movieTitle = moviesService.GetMovieTitleById(movieId);

                ConfirmWindow _popupConfirm = new ConfirmWindow("Are you sure you want to add a view to " + (!String.IsNullOrEmpty(movieTitle) ? movieTitle : "this movie") + "?");

                if (_popupConfirm.ShowDialog() == true)
                {
                    if (moviesService.AddMovieViewById(movieId) == OutputTypeValues.Ok)
                    {
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Successfully added movie view with id " + movieId.ToString() + "!");
                    }
                    else
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Error occurred trying adding movie view with id " + movieId.ToString() + "!");
                }

            }
        }

        private void DeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewMovies.SelectedItem != null)
            {
                ConfirmWindow _popupConfirm = new ConfirmWindow("Are you sure you want to delete this movie?");

                if (_popupConfirm.ShowDialog() == true)
                {
                    if (moviesService.DeleteMovieById(((MoviesViewModel)ListViewMovies.SelectedItem).MovieId) == OutputTypeValues.Ok)
                    {
                        ReloadMoviesList();
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Successfully deleted movie with id " + ((MoviesViewModel)ListViewMovies.SelectedItem).MovieId.ToString() + "!");
                    }
                    else
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Error occurred trying delete movie with id " + ((MoviesViewModel)ListViewMovies.SelectedItem).MovieId.ToString() + "!");
                }

            }
        }        

        private void ButtonSearchMovies_Click(object sender, RoutedEventArgs e)
        {
            SearchMoviesList();
        }

        private void ButtonClearSearch_Click(object sender, RoutedEventArgs e)
        {
            //Clear filters
            TextBoxSearchTerm.Clear();
            ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0);
            CheckBoxIsFavorite.IsChecked = false;
            ComboBoxRatings.SelectedItem = ComboBoxRatings.Items.GetItemAt(0);

            searchTitle = null;
            searchYear = null;
            searchFavorite = false;
            searchRating = null;

            ReloadMoviesList();
        }

        private void ButtonClearSearchYears_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxYears.SelectedValue != null)
            {
                if (ComboBoxYears.Items.IndexOf(ComboBoxYears.SelectedValue.ToString()) != 0)
                {
                    ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0);
                    ButtonClearSearchYears.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ComboBoxYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxYears.SelectedValue != null)
            {
                if (ComboBoxYears.Items.IndexOf(ComboBoxYears.SelectedValue.ToString()) != 0)
                {
                    ButtonClearSearchYears.Visibility = Visibility.Visible;
                }
                else
                {
                    //Clear combobox filter
                    ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0);
                    ButtonClearSearchYears.Visibility = Visibility.Hidden;
                }
            }            
        }

        private void ComboBoxRatings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxRatings.SelectedValue != null)
            {
                if (ComboBoxRatings.Items.IndexOf(ComboBoxRatings.SelectedValue.ToString()) != 0)
                {
                    ButtonClearSearchRatings.Visibility = Visibility.Visible;
                }
                else
                {
                    //Clear combobox filter
                    ComboBoxRatings.SelectedItem = ComboBoxYears.Items.GetItemAt(0);
                    ButtonClearSearchRatings.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ButtonClearSearchRatings_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxRatings.SelectedValue != null)
            {
                if (ComboBoxRatings.Items.IndexOf(ComboBoxRatings.SelectedValue.ToString()) != 0)
                {
                    ComboBoxRatings.SelectedItem = ComboBoxRatings.Items.GetItemAt(0);
                    ButtonClearSearchRatings.Visibility = Visibility.Hidden;
                }
            }
        }     

        private void TextBoxSearchTerm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchMoviesList();
            }
        }


        //Pagination
        private void LoadPaginationForPage(int totalItems)
        {       
            //Total de páginas
            pagLastNumber = totalItems / IPP;
            if (totalItems % IPP != 0)
                pagLastNumber += 1;

            if (pagLastNumber == 0) pagLastNumber = 1;
        }

        private void Button_Pag_Left_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber - 1) >= 1)
            {
                pagNumber -= 1;
                ReloadMoviesList();
            }
        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != 1)
            {
                pagNumber = 1;
                ReloadMoviesList();
            }
        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != pagLastNumber)
            {
                pagNumber = pagLastNumber;
                ReloadMoviesList();
            }
        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber + 1) <= pagLastNumber)
            {
                pagNumber += 1;
                ReloadMoviesList();
            }
        }
    }
}
