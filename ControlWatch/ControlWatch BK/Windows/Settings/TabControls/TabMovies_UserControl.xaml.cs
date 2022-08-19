using ControlWatch.Commons.Helpers;
using ControlWatch.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

namespace ControlWatch.Windows.Settings.TabControls
{
    /// <summary>
    /// Interaction logic for TabMovies_UserControl.xaml
    /// </summary>
    public partial class TabMovies_UserControl : UserControl
    {
        private MovieService movieService;

        //Pagination
        private int pagNumber = 1;
        private int pagLastNumber = 1;
        private int IPP = 50;

        public TabMovies_UserControl()
        {
            UtilsOperations.StartLoadingAnimation();

            InitializeComponent();
            movieService = new MovieService();

            LoadMoviesData();
        }

        private void LoadMoviesData()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var moviesList = movieService.GetAllMovies(((pagNumber - 1) * IPP), IPP);

                if (moviesList != null && moviesList.Any())
                {
                    int? nMovies = movieService.GetMoviesCount(null, null, false, null, true);
                    LoadPaginationForPage(nMovies.HasValue ? nMovies.Value : moviesList.Count());

                    DataGridMovies.Dispatcher.BeginInvoke((Action)(() => DataGridMovies.ItemsSource = null));
                    ObservableCollection<MoviesGridItem> moviesToGrid = new ObservableCollection<MoviesGridItem>();

                    foreach (var item in moviesList)
                    {
                        moviesToGrid.Add(new MoviesGridItem()
                        {
                            MovieId = item.MovieId.ToString(),
                            MovieTitle = item.MovieTitle,
                            MovieYear = item.MovieYear.ToString(),
                            NrViews = item.NrViews.ToString(),
                            MovieRating = item.MovieRating.ToString(),
                            IsFavorite = item.IsFavorite ? "/ControlWatch;component/Resources/favorites-icon.png" : null,
                            CreateDate = item.CreateDate.ToString("dd-MM-yyyy HH:mm"),
                            Deleted = item.Deleted ? "1" : "0",
                            Observations = item.Observations
                        });
                    }

                    //BINDING
                    DataGridMovies.Dispatcher.BeginInvoke((Action)(() => DataGridMovies.ItemsSource = moviesToGrid));

                    //NUMBER
                    LabelTotals.Dispatcher.BeginInvoke((Action)(() => ShowPaginationText()));
                }

                UtilsOperations.StopLoadingAnimation();

            }).Start();            
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

        private void ShowPaginationText()
        {
            LabelTotals.Content = pagLastNumber == 0 ? "0" : pagNumber.ToString();
            LabelTotals.Content += " of " + pagLastNumber + ((pagLastNumber > 1 || pagLastNumber == 0) ? " pages" : " page");

            var nrMovies = movieService.GetAllMoviesCount();
            if(nrMovies != null)
            {
                LabelTotals.Content += "   Total: " + nrMovies.Item1.ToString() + (nrMovies.Item1 == 1 ? " movie (" : " movies (")
                    + nrMovies.Item2.ToString() + " deleted)";
            }
        }

        private void Button_Pag_Left_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber - 1) >= 1)
            {
                pagNumber -= 1;
                LoadMoviesData();
            }
        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != 1)
            {
                pagNumber = 1;
                LoadMoviesData();
            }
        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != pagLastNumber)
            {
                pagNumber = pagLastNumber;
                LoadMoviesData();
            }
        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber + 1) <= pagLastNumber)
            {
                pagNumber += 1;
                LoadMoviesData();
            }
        }
    }

    public class MoviesGridItem
    {
        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string MovieYear { get; set; }
        public string NrViews { get; set; }
        public string IsFavorite { get; set; }
        public string MovieRating { get; set; }
        public string CreateDate { get; set; }
        public string Deleted { get; set; }
        public string Observations { get; set; }
    }
}
