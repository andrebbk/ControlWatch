using ControlWatch.Commons.Helpers;
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

namespace ControlWatch.Windows.Settings.TabControls
{
    /// <summary>
    /// Interaction logic for TabTvShows_UserControl.xaml
    /// </summary>
    public partial class TabTvShows_UserControl : UserControl
    {
        private TvShowService tvShowService;

        //Pagination
        private int pagNumber = 1;
        private int pagLastNumber = 1;
        private int IPP = 50;

        public TabTvShows_UserControl()
        {
            UtilsOperations.StartLoadingAnimation();

            InitializeComponent();
            tvShowService = new TvShowService();

            LoadTvShowsData();
        }


        private void LoadTvShowsData()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var tvShowsList = tvShowService.GetAllTvShows(((pagNumber - 1) * IPP), IPP);

                if (tvShowsList != null && tvShowsList.Any())
                {
                    int? nTvShows = tvShowService.GetTvShowsCount(null, null, false, null, false, true);
                    LoadPaginationForPage(nTvShows.HasValue ? nTvShows.Value : tvShowsList.Count());

                    DataGridTvShows.Dispatcher.BeginInvoke((Action)(() => DataGridTvShows.ItemsSource = null));
                    ObservableCollection<TvShowsGridItem> tvShowsToGrid = new ObservableCollection<TvShowsGridItem>();

                    foreach (var item in tvShowsList)
                    {
                        tvShowsToGrid.Add(new TvShowsGridItem()
                        {
                            TvShowId = item.TvShowId.ToString(),
                            TvShowTitle = item.TvShowTitle,
                            TvShowYear = item.TvShowYear.ToString(),
                            NrViews = item.NrViews.ToString(),
                            TvShowSeasons = item.TvShowSeasons.ToString(),
                            TvShowEpisodes = item.TvShowEpisodes.ToString(),
                            IsFavorite = item.IsFavorite ? "/ControlWatch;component/Resources/favorites-icon.png" : null,
                            TvShowRating = item.TvShowRating.ToString(),                           
                            IsFinished = item.IsFinished ? "/ControlWatch;component/Resources/Buttons/checkwhite.png" : null,
                            CreateDate = item.CreateDate.ToString("dd-MM-yyyy HH:mm"),
                            Deleted = item.Deleted ? "1" : "0",
                            Observations = item.Observations
                        });
                    }

                    //BINDING
                    DataGridTvShows.Dispatcher.BeginInvoke((Action)(() => DataGridTvShows.ItemsSource = tvShowsToGrid));

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

            var nrTvShows = tvShowService.GetAllTvShowsCount();
            if (nrTvShows != null)
            {
                LabelTotals.Content += "   Total: " + nrTvShows.Item1.ToString() + (nrTvShows.Item1 == 1 ? " tv show (" : " tv shows (")
                    + nrTvShows.Item2.ToString() + " deleted)";
            }
        }

        private void Button_Pag_Left_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber - 1) >= 1)
            {
                pagNumber -= 1;
                LoadTvShowsData();
            }
        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != 1)
            {
                pagNumber = 1;
                LoadTvShowsData();
            }
        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != pagLastNumber)
            {
                pagNumber = pagLastNumber;
                LoadTvShowsData();
            }
        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber + 1) <= pagLastNumber)
            {
                pagNumber += 1;
                LoadTvShowsData();
            }
        }
    }

    public class TvShowsGridItem
    {
        public string TvShowId { get; set; }
        public string TvShowTitle { get; set; }
        public string TvShowYear { get; set; }
        public string TvShowSeasons { get; set; }
        public string TvShowEpisodes { get; set; }
        public string NrViews { get; set; }
        public string IsFavorite { get; set; }
        public string TvShowRating { get; set; }
        public string IsFinished { get; set; }
        public string CreateDate { get; set; }
        public string Deleted { get; set; }
        public string Observations { get; set; }
    }
}
