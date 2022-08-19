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

namespace ControlWatch.Windows.Dashboard
{
    /// <summary>
    /// Interaction logic for Dashboard_UserControl.xaml
    /// </summary>
    public partial class Dashboard_UserControl : UserControl
    {       
        private MainWindow _mainWindow;

        private StatsService statsService;

        public Dashboard_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
            statsService = new StatsService();

            LoadDashboard();
        }

        private void LoadDashboard()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var statsData = statsService.GetStats();

                if (statsData != null)
                {
                    if (!String.IsNullOrEmpty(statsData.MoviesCount) && !String.IsNullOrEmpty(statsData.MoviesViewsCount))
                    {
                        //BINDING
                        LabelMoviesCount.Dispatcher.BeginInvoke((Action)(() => LabelMoviesCount.Content = "Movies: " + statsData.MoviesCount));
                        LabelMoviesViewsCount.Dispatcher.BeginInvoke((Action)(() => LabelMoviesViewsCount.Content = "Views: " + statsData.MoviesViewsCount));
                    }

                    if (!String.IsNullOrEmpty(statsData.TvShowsCount) && !String.IsNullOrEmpty(statsData.TvShowsViewsCount))
                    {
                        //BINDING
                        LabelTvShowsCount.Dispatcher.BeginInvoke((Action)(() => LabelTvShowsCount.Content = "Tv Shows: " + statsData.TvShowsCount));
                        LabelTvShowsViewsCount.Dispatcher.BeginInvoke((Action)(() => LabelTvShowsViewsCount.Content = "Views: " + statsData.TvShowsViewsCount));
                    }
                }

                UtilsOperations.StopLoadingAnimation();

            }).Start();
        }
    }
}
