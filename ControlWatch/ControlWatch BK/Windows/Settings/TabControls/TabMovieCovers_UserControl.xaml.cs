using ControlWatch.Commons.Helpers;
using ControlWatch.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ControlWatch.Windows.Settings.TabControls
{
    /// <summary>
    /// Interaction logic for TabMovieCovers_UserControl.xaml
    /// </summary>
    public partial class TabMovieCovers_UserControl : UserControl
    {
        private MovieService movieService;

        //Pagination
        private int pagNumber = 1;
        private int pagLastNumber = 1;
        private int IPP = 50;

        public TabMovieCovers_UserControl()
        {
            InitializeComponent();
            movieService = new MovieService();

            LoadMovieCoversData();
        }

        public void LoadMovieCoversData()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var movieCoversList = movieService.GetAllMovieCovers(((pagNumber - 1) * IPP), IPP);

                if (movieCoversList != null && movieCoversList.Any())
                {
                    var nMoviesCovers = movieService.GetAllMovieCoversCount();
                    LoadPaginationForPage(nMoviesCovers != null && nMoviesCovers.Item1 > 0 ? nMoviesCovers.Item1 : movieCoversList.Count());

                    DataGridMovieCovers.Dispatcher.BeginInvoke((Action)(() => DataGridMovieCovers.ItemsSource = null));
                    ObservableCollection<MovieCoversGridItem> movieCoversToGrid = new ObservableCollection<MovieCoversGridItem>();

                    foreach (var item in movieCoversList)
                    {
                        movieCoversToGrid.Add(new MovieCoversGridItem()
                        {
                            MovieId = item.MovieId.ToString(),
                            MovieTitle = item.MovieTitle,
                            MovieCoverId = item.MovieCoverId.ToString(),
                            CoverName = item.CoverName,
                            CoverPath = item.CoverPath,
                            CreateDate = item.CreateDate.ToString("dd-MM-yyyy HH:mm"),
                            Deleted = item.Deleted ? "1" : "0"
                        });
                    }

                    //BINDING
                    DataGridMovieCovers.Dispatcher.BeginInvoke(
                        (Action)(() => {
                            DataGridMovieCovers.ItemsSource = movieCoversToGrid;
                        }));

                    //NUMBER
                    LabelTotals.Dispatcher.BeginInvoke((Action)(() => ShowPaginationText(nMoviesCovers)));
                }

                UtilsOperations.StopLoadingAnimation();

            }).Start();
        }        


        //Buttons
        private void DataGridMovieCovers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridMovieCovers.SelectedItem == null) return;

            var selectedPerson = DataGridMovieCovers.SelectedItem as MovieCoversGridItem;

            if(selectedPerson != null && !String.IsNullOrEmpty(selectedPerson.CoverPath))
            {
                if (File.Exists(selectedPerson.CoverPath))
                {
                    //Show cover with windows photo default program
                    Process.Start("explorer.exe", selectedPerson.CoverPath);
                }
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

        private void ShowPaginationText(Tuple<int, int> nrMovieCovers)
        {
            LabelTotals.Content = pagLastNumber == 0 ? "0" : pagNumber.ToString();
            LabelTotals.Content += " of " + pagLastNumber + ((pagLastNumber > 1 || pagLastNumber == 0) ? " pages" : " page");
            
            if (nrMovieCovers != null)
            {
                LabelTotals.Content += "   Total: " + nrMovieCovers.Item1.ToString() + (nrMovieCovers.Item1 == 1 ? " movie cover (" : " movie covers (")
                    + nrMovieCovers.Item2.ToString() + " deleted)";
            }
        }

        private void Button_Pag_Left_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber - 1) >= 1)
            {
                pagNumber -= 1;
                LoadMovieCoversData();
            }
        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != 1)
            {
                pagNumber = 1;
                LoadMovieCoversData();
            }
        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != pagLastNumber)
            {
                pagNumber = pagLastNumber;
                LoadMovieCoversData();
            }
        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber + 1) <= pagLastNumber)
            {
                pagNumber += 1;
                LoadMovieCoversData();
            }
        }        
    }

    public class MovieCoversGridItem
    {
        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string MovieCoverId { get; set; }
        public string CoverName { get; set; }
        public string CoverPath { get; set; }
        public string CreateDate { get; set; }
        public string Deleted { get; set; }
    }
}
