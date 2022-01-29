using ControlWatch.Commons.Helpers;
using ControlWatch.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for TabTvShowsCoversUserControl.xaml
    /// </summary>
    public partial class TabTvShowsCoversUserControl : UserControl
    {
        private TvShowService tvShowService;

        //Pagination
        private int pagNumber = 1;
        private int pagLastNumber = 1;
        private int IPP = 50;

        public TabTvShowsCoversUserControl()
        {
            InitializeComponent();
            tvShowService = new TvShowService();

            LoadTvShowsCoversData();
        }

        public void LoadTvShowsCoversData()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var tvShowsCoversList = tvShowService.GetAllTvShowsCovers(((pagNumber - 1) * IPP), IPP);

                if (tvShowsCoversList != null && tvShowsCoversList.Any())
                {
                    var nTvShowsCovers = tvShowService.GetAllTvShowsCoversCount();
                    LoadPaginationForPage(nTvShowsCovers != null && nTvShowsCovers.Item1 > 0 ? nTvShowsCovers.Item1 : tvShowsCoversList.Count());

                    DataGridTvShowCovers.Dispatcher.BeginInvoke((Action)(() => DataGridTvShowCovers.ItemsSource = null));
                    ObservableCollection<TvShowsCoversGridItem> tvShowsCoversToGrid = new ObservableCollection<TvShowsCoversGridItem>();

                    foreach (var item in tvShowsCoversList)
                    {
                        tvShowsCoversToGrid.Add(new TvShowsCoversGridItem()
                        {
                            TvShowId = item.TvShowId.ToString(),
                            TvShowTitle = item.TvShowTitle,
                            TvShowCoverId = item.TvShowCoverId.ToString(),
                            CoverName = item.CoverName,
                            CoverPath = item.CoverPath,
                            CreateDate = item.CreateDate.ToString("dd-MM-yyyy HH:mm"),
                            Deleted = item.Deleted ? "1" : "0"
                        });
                    }

                    //BINDING
                    DataGridTvShowCovers.Dispatcher.BeginInvoke(
                        (Action)(() => {
                            DataGridTvShowCovers.ItemsSource = tvShowsCoversToGrid;
                        }));

                    //NUMBER
                    LabelTotals.Dispatcher.BeginInvoke((Action)(() => ShowPaginationText(nTvShowsCovers)));
                }

                UtilsOperations.StopLoadingAnimation();

            }).Start();
        }


        //Buttons
        private void DataGridTvShowCovers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridTvShowCovers.SelectedItem == null) return;

            var selectedPerson = DataGridTvShowCovers.SelectedItem as TvShowsCoversGridItem;

            if (selectedPerson != null && !String.IsNullOrEmpty(selectedPerson.CoverPath))
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
                LabelTotals.Content += "   Total: " + nrMovieCovers.Item1.ToString() + (nrMovieCovers.Item1 == 1 ? " tv show cover (" : " tv shows covers (")
                    + nrMovieCovers.Item2.ToString() + " deleted)";
            }
        }

        private void Button_Pag_Left_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber - 1) >= 1)
            {
                pagNumber -= 1;
                LoadTvShowsCoversData();
            }
        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != 1)
            {
                pagNumber = 1;
                LoadTvShowsCoversData();
            }
        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != pagLastNumber)
            {
                pagNumber = pagLastNumber;
                LoadTvShowsCoversData();
            }
        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber + 1) <= pagLastNumber)
            {
                pagNumber += 1;
                LoadTvShowsCoversData();
            }
        }        
    }

    public class TvShowsCoversGridItem
    {
        public string TvShowId { get; set; }
        public string TvShowTitle { get; set; }
        public string TvShowCoverId { get; set; }
        public string CoverName { get; set; }
        public string CoverPath { get; set; }
        public string CreateDate { get; set; }
        public string Deleted { get; set; }
    }
}
