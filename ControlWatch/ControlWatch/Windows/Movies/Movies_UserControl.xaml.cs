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
                var moviesList = moviesService.GetMovies(searchTitle, searchYear, searchFavorite);              

                if (moviesList != null && moviesList.Any())
                {
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

                    //load filter
                    LoadMoviesFilter();
                }
            }).Start();
        }

        private void LoadMoviesFilter()
        {            
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
        }

        private void ReloadMoviesList()
        {
            new Thread(() =>
            {
                var moviesList = moviesService.GetMovies(searchTitle, searchYear, searchFavorite);

                if (moviesList != null)
                {
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
            }).Start();
        }

        //Event Actions
        private void ListViewMovies_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ListViewMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OpenMovie_Click(object sender, RoutedEventArgs e)
        {

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

        private void Button_Pag_Left_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSearchMovies_Click(object sender, RoutedEventArgs e)
        {
            //Load filters
            searchTitle = TextBoxSearchTerm.Text;
            searchYear = null;
            searchFavorite = CheckBoxIsFavorite.IsChecked.Value;

            if (ComboBoxYears.SelectedValue != null)
            {
                int outYear = 0;
                if (int.TryParse(ComboBoxYears.SelectedValue.ToString(), out outYear))
                {
                    searchYear = outYear;
                }
            }

            if (!String.IsNullOrEmpty(searchTitle) || searchYear != null || searchFavorite)
            {     
                ReloadMoviesList();
            }                
        }

        private void ButtonClearSearch_Click(object sender, RoutedEventArgs e)
        {
            //Clear filters
            TextBoxSearchTerm.Clear();
            ComboBoxYears.SelectedItem = ComboBoxYears.Items.GetItemAt(0);
            CheckBoxIsFavorite.IsChecked = false;

            searchTitle = null;
            searchYear = null;
            searchFavorite = false;

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
    }
}
