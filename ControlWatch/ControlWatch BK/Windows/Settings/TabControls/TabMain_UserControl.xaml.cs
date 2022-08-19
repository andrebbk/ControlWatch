using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Notifications.CustomMessage;
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
    /// Interaction logic for TabMain_UserControl.xaml
    /// </summary>
    public partial class TabMain_UserControl : UserControl
    {
        private ConfigurationService configService;
        private OperationTypeValues currentOperation;

        public TabMain_UserControl()
        {
            InitializeComponent();
            configService = new ConfigurationService();
            currentOperation = OperationTypeValues.Info;

            LoadMainSettings();
        }

        private void LoadMainSettings()
        {
            new Thread(() =>
            {
                UtilsOperations.StartLoadingAnimation();

                var pathConfigs = configService.GetCurrentPathConfig();
                if(pathConfigs != null)
                {
                    TextBox_MoviesCoverPath.Dispatcher.BeginInvoke((Action)(() => TextBox_MoviesCoverPath.Text = pathConfigs.MoviesCoversPathConfig));
                    TextBox_TvShowsCoverPath.Dispatcher.BeginInvoke((Action)(() => TextBox_TvShowsCoverPath.Text = pathConfigs.TvShowsCoversPathConfig));
                }

                LoadDataGridConfigurations();

                UtilsOperations.StopLoadingAnimation();

            }).Start();
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (String.IsNullOrWhiteSpace(TextBox_MoviesCoverPath.Text))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movies cover path is required!");
                isValid = false;
            }
            else if (String.IsNullOrWhiteSpace(TextBox_TvShowsCoverPath.Text))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Tv Shows cover path is required!");
                isValid = false;
            }

            return isValid;
        }

        private void ManageOperationControllers()
        {
            if(currentOperation == OperationTypeValues.Info)
            {
                //edit config
                currentOperation = OperationTypeValues.Edit;

                ButtonSaveConfiguration.Dispatcher.BeginInvoke((Action)(() => ButtonSaveConfiguration.Visibility = Visibility.Visible));
                ButtonMoviesLoadPath.Dispatcher.BeginInvoke((Action)(() => ButtonMoviesLoadPath.Visibility = Visibility.Visible));
                ButtonTvShowsLoadPath.Dispatcher.BeginInvoke((Action)(() => ButtonTvShowsLoadPath.Visibility = Visibility.Visible));
            }
            else
            {
                //info
                currentOperation = OperationTypeValues.Info;

                ButtonSaveConfiguration.Dispatcher.BeginInvoke((Action)(() => ButtonSaveConfiguration.Visibility = Visibility.Hidden));
                ButtonMoviesLoadPath.Dispatcher.BeginInvoke((Action)(() => ButtonMoviesLoadPath.Visibility = Visibility.Hidden));
                ButtonTvShowsLoadPath.Dispatcher.BeginInvoke((Action)(() => ButtonTvShowsLoadPath.Visibility = Visibility.Hidden));
            }
        }

        private void LoadDataGridConfigurations()
        {
            var configsList = configService.GetAllConfigurations();

            if (configsList != null && configsList.Any())
            {
                DataGridConfigurations.Dispatcher.BeginInvoke((Action)(() => DataGridConfigurations.ItemsSource = null));
                ObservableCollection<ConfigurationsGridItem> configurationsToGrid = new ObservableCollection<ConfigurationsGridItem>();

                foreach (var item in configsList)
                {
                    configurationsToGrid.Add(new ConfigurationsGridItem()
                    {
                        ConfigId = item.ConfigurationId.ToString(),
                        ConfigKey = item.Key,
                        ConfigValue = item.Value.ToString(),
                        CreateDate = item.CreateDate.ToString("dd-MM-yyyy HH:mm"),
                        Deleted = item.Deleted ? "1" : "0"
                    });
                }

                //BINDING
                DataGridConfigurations.Dispatcher.BeginInvoke((Action)(() => DataGridConfigurations.ItemsSource = configurationsToGrid));
            }
        }


        //Buttons
        private void ButtonEditMode_Click(object sender, RoutedEventArgs e)
        {
            ManageOperationControllers();
        }

        private void ButtonSaveConfiguration_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                new Thread(() =>
                {
                    UtilsOperations.StartLoadingAnimation();
                    var result = OutputTypeValues.Ok;

                    Application.Current.Dispatcher.Invoke((Action)(() => result = configService.CreateOrEditCoversPathConfiguration(TextBox_MoviesCoverPath.Text, TextBox_TvShowsCoverPath.Text)));

                    if (result == OutputTypeValues.Ok)
                    {
                        LoadDataGridConfigurations();

                        UtilsOperations.StopLoadingAnimation();
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Covers Path Configuration succefully updated!");
                        ManageOperationControllers();
                    }
                    else
                    {
                        UtilsOperations.StopLoadingAnimation();
                        NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Error occurred updating Covers Path Configuration!");
                    }

                }).Start();                               
            }            
        }

        private void ButtonMoviesLoadPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                TextBox_MoviesCoverPath.Text = dialog.SelectedPath + @"\";
            }
        }

        private void ButtonTvShowsLoadPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                TextBox_TvShowsCoverPath.Text = dialog.SelectedPath + @"\";
            }
        }
    }

    public class ConfigurationsGridItem
    {
        public string ConfigId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string CreateDate { get; set; }
        public string Deleted { get; set; }
    }
}
