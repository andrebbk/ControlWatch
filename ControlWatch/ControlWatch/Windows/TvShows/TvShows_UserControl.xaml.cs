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

namespace ControlWatch.Windows.TvShows
{
    /// <summary>
    /// Interaction logic for TvShows_UserControl.xaml
    /// </summary>
    public partial class TvShows_UserControl : UserControl
    {
        private MainWindow _mainWindow;
        private TvShowService tvShowService;

        private string searchTitle = null;
        private int? searchYear = null;
        private bool searchFavorite = false;
        private int? searchRating = null;

        //Pagination
        private int pagNumber = 1;
        private int pagLastNumber = 1;
        private int IPP = 200;

        public TvShows_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            this.tvShowService = new TvShowService();

            LoadTvShowList();
        }

        private void LoadTvShowList()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var tvShowsList = tvShowService.GetTvShows(((pagNumber - 1) * IPP), IPP, searchTitle, searchYear, searchFavorite, searchRating);

                if (tvShowsList != null && tvShowsList.Any())
                {
                    int? nTvShow = tvShowService.GetTvShowsCount(searchTitle, searchYear, searchFavorite, searchRating);
                    LoadPaginationForPage(nTvShow.HasValue ? nTvShow.Value : tvShowsList.Count());

                    ListViewTvShows.Dispatcher.BeginInvoke((Action)(() => ListViewTvShows.ItemsSource = null));
                    ObservableCollection<TvShowsViewModel> tvShowsToShow = new ObservableCollection<TvShowsViewModel>();

                    foreach (var item in tvShowsList)
                    {
                        //load imagem
                        if (!String.IsNullOrEmpty(item.TvShowCoverPath))
                            item.TvShowCover = Utils.LoadImageToBitmapStreamImage(item.TvShowCoverPath);

                        tvShowsToShow.Add(item);
                    }

                    //BINDING
                    ListViewTvShows.Dispatcher.BeginInvoke((Action)(() => ListViewTvShows.ItemsSource = tvShowsToShow));

                    //RESTART SCROLLBAR
                    ListViewTvShows.Dispatcher.BeginInvoke((Action)(() => ListViewTvShows.ScrollIntoView(ListViewTvShows.Items[0])));
                }

                //load filter
                LoadTvShowsFilter();

                UtilsOperations.StopLoadingAnimation();

            }).Start();
        }

        private void LoadTvShowsFilter()
        {
            //TvShow year filter
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

            //TvShow rating filter
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

        private void ReloadTvShowsList()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var tvShowsList = tvShowService.GetTvShows(((pagNumber - 1) * IPP), IPP, searchTitle, searchYear, searchFavorite, searchRating);

                if (tvShowsList != null)
                {
                    int? nTvShows = tvShowService.GetTvShowsCount(searchTitle, searchYear, searchFavorite, searchRating);
                    LoadPaginationForPage(nTvShows.HasValue ? nTvShows.Value : tvShowsList.Count());

                    ListViewTvShows.Dispatcher.BeginInvoke((Action)(() => ListViewTvShows.ItemsSource = null));
                    ObservableCollection<TvShowsViewModel> tvShowsToShow = new ObservableCollection<TvShowsViewModel>();

                    foreach (var item in tvShowsList)
                    {
                        //load imagem
                        if (!String.IsNullOrEmpty(item.TvShowCoverPath))
                            item.TvShowCover = Utils.LoadImageToBitmapStreamImage(item.TvShowCoverPath);

                        tvShowsToShow.Add(item);
                    }

                    //BINDING
                    ListViewTvShows.Dispatcher.BeginInvoke((Action)(() => ListViewTvShows.ItemsSource = tvShowsToShow));

                    //RESTART SCROLLBAR
                    if (tvShowsToShow.Any())
                        ListViewTvShows.Dispatcher.BeginInvoke((Action)(() => ListViewTvShows.ScrollIntoView(ListViewTvShows.Items[0])));
                }

                UtilsOperations.StopLoadingAnimation();

            }).Start();
        }

        private void SearchTvShowsList()
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
                ReloadTvShowsList();
            }
        }


        //Button's Action
        private void ListViewTvShows_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewTvShows.SelectedItem != null)
            {
                //_mainWindow.SetMainContent(MenuOptionsTypeValues.TvShowInfo, ((TvShowsViewModel)ListViewTvShows.SelectedItem).TvShowId);
            }
        }

        private void OpenTvShow_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewTvShows.SelectedItem != null)
            {
                //_mainWindow.SetMainContent(MenuOptionsTypeValues.TvShowInfo, ((TvShowsViewModel)ListViewTvShows.SelectedItem).TvShowId);
            }
        }

        private void AddView_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewTvShows.SelectedItem != null)
            {
                int tvShowId = ((TvShowsViewModel)ListViewTvShows.SelectedItem).TvShowId;
                string tvShowTitle = tvShowService.GetTvShowTitleById(tvShowId);

                ConfirmWindow _popupConfirm = new ConfirmWindow("Are you sure you want to add a view to " + (!String.IsNullOrEmpty(tvShowTitle) ? tvShowTitle : "this tv show") + "?");

                if (_popupConfirm.ShowDialog() == true)
                {
                    if (tvShowService.AddTvShowViewById(tvShowId) == OutputTypeValues.Ok)
                    {
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Successfully added tv show view with id " + tvShowId.ToString() + "!");
                    }
                    else
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Error occurred trying adding tv show view with id " + tvShowId.ToString() + "!");
                }

            }
        }

        private void DeleteTvShow_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewTvShows.SelectedItem != null)
            {
                int tvShowId = ((TvShowsViewModel)ListViewTvShows.SelectedItem).TvShowId;
                ConfirmWindow _popupConfirm = new ConfirmWindow("Are you sure you want to delete this tv show?");

                if (_popupConfirm.ShowDialog() == true)
                {
                    if (tvShowService.DeleteTvShowById(tvShowId) == OutputTypeValues.Ok)
                    {
                        ReloadTvShowsList();
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Successfully deleted tv show with id " + tvShowId.ToString() + "!");
                    }
                    else
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Error occurred trying delete tv show with id " + tvShowId.ToString() + "!");
                }

            }
        }

        private void ButtonSearchTvShows_Click(object sender, RoutedEventArgs e)
        {
            SearchTvShowsList();
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

            ReloadTvShowsList();
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
            if (ComboBoxYears.SelectedValue != null)
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
                SearchTvShowsList();
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
                ReloadTvShowsList();
            }
        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != 1)
            {
                pagNumber = 1;
                ReloadTvShowsList();
            }
        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {
            if (pagNumber != pagLastNumber)
            {
                pagNumber = pagLastNumber;
                ReloadTvShowsList();
            }
        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {
            if ((pagNumber + 1) <= pagLastNumber)
            {
                pagNumber += 1;
                ReloadTvShowsList();
            }
        }
    }
}
