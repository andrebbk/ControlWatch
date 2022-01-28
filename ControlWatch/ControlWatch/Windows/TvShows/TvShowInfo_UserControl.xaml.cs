using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Notifications.CustomMessage;
using ControlWatch.Popup;
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

namespace ControlWatch.Windows.TvShows
{
    /// <summary>
    /// Interaction logic for TvShowInfo_UserControl.xaml
    /// </summary>
    public partial class TvShowInfo_UserControl : UserControl
    {
        private MainWindow _mainWindow;
        private TvShowService tvShowService;

        private int LoadedTvShowId;
        private OperationTypeValues currentMode;
        private string LoadedTvShowCoverPath = null;
        private int NewTvShowYear = 0, NewTvShowViews = 0;

        public TvShowInfo_UserControl(MainWindow mainWindow, int? tvShowId)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            tvShowService = new TvShowService();
            currentMode = OperationTypeValues.Info;

            if (tvShowId.HasValue && tvShowId.Value > 0)
                LoadTvShowInfo(tvShowId.Value);
        }


        private void LoadTvShowInfo(int tvShowId)
        {
            new Thread(() =>
            {
                if (currentMode == OperationTypeValues.Edit) ResetInfoMode();

                //load param
                LoadedTvShowId = tvShowId;
                currentMode = OperationTypeValues.Info;
                LoadedTvShowCoverPath = null;
                NewTvShowYear = NewTvShowViews = 0;

                var tvShowInfo = tvShowService.GetTvShowById(tvShowId);
                if (tvShowInfo != null)
                {
                    //TvShow title
                    TextBox_TvShowTitle.Dispatcher.BeginInvoke((Action)(() => TextBox_TvShowTitle.Text = tvShowInfo.TvShowTitle));

                    //TvShow year
                    var currentYear = DateTime.Now.Year;
                    List<string> yearsList = new List<string>();

                    for (int y = currentYear; y > 1979; y--)
                        yearsList.Add(y.ToString());

                    if (yearsList != null && yearsList.Any())
                    {
                        ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.ItemsSource = yearsList));

                        ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.SelectedItem = tvShowInfo.TvShowYear.ToString()));
                    }

                    //TvShow Seasons & Episodes
                    UpDownSeasons.Dispatcher.BeginInvoke((Action)(() => UpDownSeasons.Text = tvShowInfo.TvShowSeasons.ToString()));
                    UpDownEpisodes.Dispatcher.BeginInvoke((Action)(() => UpDownEpisodes.Text = tvShowInfo.TvShowEpisodes.ToString()));

                    //TvShow Favorite
                    if (tvShowInfo.IsFavorite)
                        CheckBoxIsFavorite.Dispatcher.BeginInvoke((Action)(() => CheckBoxIsFavorite.IsChecked = tvShowInfo.IsFavorite));

                    //Load TvShow cover
                    if (!String.IsNullOrEmpty(tvShowInfo.CoverPath))
                    {
                        TextBox_TvShowCoverFileName.Dispatcher.BeginInvoke((Action)(() => TextBox_TvShowCoverFileName.Text = tvShowInfo.CoverName));
                        TvShowCover.Dispatcher.BeginInvoke((Action)(() => TvShowCover.Source = Utils.LoadImageToBitmapStreamImage(tvShowInfo.CoverPath)));
                    }

                    //TvShow views
                    List<string> viewsList = new List<string>();
                    for (int y = 0; y < 101; y++)
                        viewsList.Add(y.ToString());

                    if (viewsList != null && viewsList.Any())
                    {
                        ComboBoxViews.Dispatcher.BeginInvoke((Action)(() => ComboBoxViews.ItemsSource = viewsList));

                        ComboBoxViews.Dispatcher.BeginInvoke((Action)(() => ComboBoxViews.SelectedItem = tvShowInfo.NrViews.ToString()));
                    }

                    //Create date
                    LabelTvShowAdded.Dispatcher.BeginInvoke((Action)(() => LabelTvShowAdded.Content = tvShowInfo.CreateDate.ToString()));

                    //Rating TvShow
                    double ratingValue = (double)tvShowInfo.TvShowRating / (double)10;
                    RatingTvShow.Dispatcher.BeginInvoke((Action)(() => RatingTvShow.Value = ratingValue));

                    //Observations
                    TextBox_Observations.Dispatcher.BeginInvoke((Action)(() => TextBox_Observations.Text = tvShowInfo.Observations));
                }

            }).Start();
        }

        private bool ValidateModel()
        {
            bool isValid = true;

            if (String.IsNullOrWhiteSpace(TextBox_TvShowTitle.Text))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow title is required!");
                isValid = false;
            }
            else if (!int.TryParse(ComboBoxYears.SelectedValue.ToString(), out NewTvShowYear))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow year is invalid!");
                isValid = false;
            }
            else if (NewTvShowYear < 1980)
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow year is invalid!");
                isValid = false;
            }
            else if (!int.TryParse(ComboBoxViews.SelectedValue.ToString(), out NewTvShowViews))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow views value is invalid!");
                isValid = false;
            }
            else if (NewTvShowViews < 1)
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow views value is invalid!");
                isValid = false;
            }
            else if (!UpDownSeasons.Value.HasValue || (UpDownSeasons.Value.HasValue && (UpDownSeasons.Value.Value < 0 || UpDownSeasons.Value.Value > 200)))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow seasons is invalid!");
                isValid = false;
            }
            else if (!UpDownEpisodes.Value.HasValue || (UpDownEpisodes.Value.HasValue && (UpDownEpisodes.Value.Value < 0 || UpDownEpisodes.Value.Value > 500)))
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow episodes is invalid!");
                isValid = false;
            }

            return isValid;
        }

        private void ResetInfoMode()
        {
            currentMode = OperationTypeValues.Info;
            TextBox_TvShowTitle.Dispatcher.BeginInvoke((Action)(() => TextBox_TvShowTitle.Text = "Edit TvShow"));
            ButtonEditModeImage.Dispatcher.BeginInvoke((Action)(() =>
                ButtonEditModeImage.Source = Utils.LoadImageToBitmapFromResources("/ControlWatch;component/Resources/Buttons/Pencil.png"))
            );

            //Controls
            ButtonSaveTvShow.Dispatcher.BeginInvoke((Action)(() => ButtonSaveTvShow.Visibility = Visibility.Hidden));
            ButtonLoadPic.Dispatcher.BeginInvoke((Action)(() => ButtonLoadPic.Visibility = Visibility.Hidden));

            TextBox_TvShowTitle.Dispatcher.BeginInvoke((Action)(() => TextBox_TvShowTitle.IsEnabled = false));
            ComboBoxYears.Dispatcher.BeginInvoke((Action)(() => ComboBoxYears.IsEnabled = false));
            RatingTvShow.Dispatcher.BeginInvoke((Action)(() => RatingTvShow.IsEnabled = false));
            CheckBoxIsFavorite.Dispatcher.BeginInvoke((Action)(() => CheckBoxIsFavorite.IsEnabled = false));
            ComboBoxViews.Dispatcher.BeginInvoke((Action)(() => ComboBoxViews.IsEnabled = false));

            UpDownSeasons.Dispatcher.BeginInvoke((Action)(() => UpDownSeasons.IsEnabled = false));
            UpDownEpisodes.Dispatcher.BeginInvoke((Action)(() => UpDownEpisodes.IsEnabled = false));

            TextBox_Observations.Dispatcher.BeginInvoke((Action)(() => TextBox_Observations.IsReadOnly = true));
        }

        private void NotifyError(OutputTypeValues result)
        {
            string msg = null;

            switch (result)
            {
                case OutputTypeValues.AlreadyExists:
                    msg = "Tv Show already exists!";
                    break;
                case OutputTypeValues.DataError:
                    msg = "Tv Show data is invalid!";
                    break;
                case OutputTypeValues.SavingCoverError:
                    msg = "An error has occurred saving cover!";
                    break;
                case OutputTypeValues.TvShowNotFound:
                    msg = "Tv Show not found!";
                    break;
                case OutputTypeValues.TvShowCoverNotFound:
                    msg = "Tv Show cover not found!";
                    break;
                case OutputTypeValues.Error:
                default:
                    msg = "An error has occurred saving tv show!";
                    break;
            }

            if (!String.IsNullOrEmpty(msg))
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", msg);
        }


        //Buttons
        private void ButtonGoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainWindow.SetMainContent(MenuOptionsTypeValues.TvShows);
        }

        private void ButtonLoadPic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
            of.Title = "Select tv show cover";
            of.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";

            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadedTvShowCoverPath = of.FileName;
                TextBox_TvShowCoverFileName.Text = of.FileName.Split('\\')[of.FileName.Split('\\').Count() - 1];

                TvShowCover.Source = Utils.LoadImageToBitmapImageNoDecodeChange(of.FileName);
            }
            else
            {
                TvShowCover.Source = null;
                LoadedTvShowCoverPath = null;
                TextBox_TvShowCoverFileName.Clear();
            }
        }

        private void ButtonSaveTvShow_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateModel())
            {
                UtilsOperations.StartLoadingAnimation();

                int ratingValue = RatingTvShow.Value.HasValue ? Convert.ToInt16((double)RatingTvShow.Value * 10) : 0;
                int nSeasons = unchecked((int)UpDownSeasons.Value.Value);
                int nEpisodes = unchecked((int)UpDownEpisodes.Value.Value);

                var editTvShowResult = tvShowService.EditTvShow(LoadedTvShowId,
                    TextBox_TvShowTitle.Text.Trim(),
                    NewTvShowYear,
                    nSeasons,
                    nEpisodes,
                    CheckBoxIsFavorite.IsChecked.Value,
                    LoadedTvShowCoverPath,
                    ratingValue,
                    NewTvShowViews,
                    TextBox_Observations.Text.Trim());

                if (editTvShowResult == OutputTypeValues.Ok)
                {
                    LoadTvShowInfo(LoadedTvShowId);
                    UtilsOperations.StopLoadingAnimation();

                    NotificationHelper.notifier.ShowCustomMessage("Control Watch", "TvShow edited successfully!");
                }
                else
                {
                    UtilsOperations.StopLoadingAnimation();
                    NotifyError(editTvShowResult);
                }
            }
        }

        private void ButtonEditMode_Click(object sender, RoutedEventArgs e)
        {
            if (currentMode == OperationTypeValues.Info)
            {
                ButtonEditModeText.Text = "Info Tv Show";
                ButtonEditModeImage.Source = Utils.LoadImageToBitmapFromResources("/ControlWatch;component/Resources/Buttons/search-silver-icon.png");
                currentMode = OperationTypeValues.Edit;

                //Controls
                ButtonSaveTvShow.Visibility = Visibility.Visible;
                ButtonLoadPic.Visibility = Visibility.Visible;

                TextBox_TvShowTitle.IsEnabled = true;
                ComboBoxYears.IsEnabled = true;
                RatingTvShow.IsEnabled = true;
                CheckBoxIsFavorite.IsEnabled = true;
                ComboBoxViews.IsEnabled = true;

                UpDownSeasons.IsEnabled = true;
                UpDownEpisodes.IsEnabled = true;

                TextBox_Observations.IsReadOnly = false;
            }
            else
            {
                ButtonEditModeText.Text = "Edit Tv Show";
                ButtonEditModeImage.Source = Utils.LoadImageToBitmapFromResources("/ControlWatch;component/Resources/Buttons/Pencil.png");
                currentMode = OperationTypeValues.Info;

                //Controls
                ButtonSaveTvShow.Visibility = Visibility.Hidden;
                ButtonLoadPic.Visibility = Visibility.Hidden;

                TextBox_TvShowTitle.IsEnabled = false;
                ComboBoxYears.IsEnabled = false;
                RatingTvShow.IsEnabled = false;
                CheckBoxIsFavorite.IsEnabled = false;
                ComboBoxViews.IsEnabled = false;

                UpDownSeasons.IsEnabled = false;
                UpDownEpisodes.IsEnabled = false;

                TextBox_Observations.IsReadOnly = true;
            }
        }        

        private void ButtonDeleteTvShow_Click(object sender, RoutedEventArgs e)
        {
            string tvShowTitle = !String.IsNullOrEmpty(TextBox_TvShowTitle.Text) ? TextBox_TvShowTitle.Text : "this tv show";
            ConfirmWindow popupConfirm = new ConfirmWindow("Are you sure you want to delete " + tvShowTitle + "?");

            if (popupConfirm.ShowDialog() == true)
            {
                if (tvShowService.DeleteTvShowById(LoadedTvShowId) == OutputTypeValues.Ok)
                {
                    NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Successfully deleted tv show with id " + LoadedTvShowId.ToString() + "!");
                    _mainWindow.SetMainContent(MenuOptionsTypeValues.TvShows);
                }
                else
                    NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Error occurred trying delete tv show with id " + LoadedTvShowId.ToString() + "!");
            }
        }
    }
}
