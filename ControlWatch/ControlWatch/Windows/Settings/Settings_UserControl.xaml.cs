using ControlWatch.Commons.Enums;
using ControlWatch.Windows.Settings.TabControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ControlWatch.Windows.Settings
{
    /// <summary>
    /// Interaction logic for Settings_UserControl.xaml
    /// </summary>
    public partial class Settings_UserControl : UserControl
    {
        private MainWindow _mainWindow;
        private SettingsTabTypeValues currentTab;

        public Settings_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            LoadSettings();
        }

        private void LoadSettings()
        {
            currentTab = SettingsTabTypeValues.MainSettings;
            ButtonTabMainSettings.Background = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131));            
        }

        private void ResetBackgroundTabButtons()
        {
            ButtonTabMainSettings.Background
                = ButtonTabMovies.Background
                = ButtonTabMoviesCovers.Background
                = ButtonTabTvShowsCovers.Background
                = ButtonTabTvShows.Background
                = new SolidColorBrush(Color.FromArgb(255, 55,54, 56));
        }


        //Buttons
        private void ButtonTabMainSettings_Click(object sender, RoutedEventArgs e)
        {
            if(currentTab != SettingsTabTypeValues.MainSettings)
            {
                ResetBackgroundTabButtons();
                ButtonTabMainSettings.Background = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131));
                currentTab = SettingsTabTypeValues.MainSettings;
            }
        }

        private void ButtonTabMovies_Click(object sender, RoutedEventArgs e)
        {
            if (currentTab != SettingsTabTypeValues.Movies)
            {
                currentTab = SettingsTabTypeValues.Movies;

                ResetBackgroundTabButtons();
                ButtonTabMovies.Background = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131));                

                SettingsContainer.Content = new TabMovies_UserControl();
            }
        }

        private void ButtonTabMoviesCovers_Click(object sender, RoutedEventArgs e)
        {
            if (currentTab != SettingsTabTypeValues.MoviesCovers)
            {
                currentTab = SettingsTabTypeValues.MoviesCovers;

                ResetBackgroundTabButtons();
                ButtonTabMoviesCovers.Background = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131));

                SettingsContainer.Content = new TabMovieCovers_UserControl();
            }
        }

        private void ButtonTabTvShows_Click(object sender, RoutedEventArgs e)
        {
            if (currentTab != SettingsTabTypeValues.TvShows)
            {
                currentTab = SettingsTabTypeValues.TvShows;

                ResetBackgroundTabButtons();
                ButtonTabTvShows.Background = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131));

                SettingsContainer.Content = new TabTvShows_UserControl();
            }
        }

        private void ButtonTabTvShowsCovers_Click(object sender, RoutedEventArgs e)
        {
            if (currentTab != SettingsTabTypeValues.TvShowsCovers)
            {
                currentTab = SettingsTabTypeValues.TvShowsCovers;

                ResetBackgroundTabButtons();
                ButtonTabTvShowsCovers.Background = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131));                

                this.SettingsContainer.Content = new TabTvShowsCoversUserControl();
            }
        }        
    }
}
