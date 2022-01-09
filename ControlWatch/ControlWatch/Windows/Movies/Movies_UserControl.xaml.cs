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

        public Movies_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
        }

        private void LoadMovieList()
        {
            ListViewMovies.IsEnabled = false;

            new Thread(() =>
            {
                var othersList = othersService.GetOthersPhotos(((pagNumber - 1) * IPP), IPP);

                var othersConfig = parameterizationService.GetParameterizationByKey(UtilsConstants.OthersRepositoryPath);

                if (othersList != null && othersConfig != null)
                {
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = null));
                    ObservableCollection<ListMoviesItem> moviesToShow = new ObservableCollection<ListMoviesItem>();

                    foreach (var item in othersList)
                        moviesToShow.Add(new ListOthersItem()
                        {
                            OtherId = item.Id,
                            OtherFile = Utils.LoadImageToBitmapStreamImage(Utils.GetGlobalRepositoryDriveLetter() + ":" + othersConfig.ParamValue + "\\" + item.FileName)
                        });

                    //BINDING
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = moviesToShow));

                    //RESTART SCROLLBAR
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ScrollIntoView(ListViewMovies.Items[0])));
                }

                //NUMBER
                //TextBlockOthersNumber.Dispatcher.BeginInvoke((Action)(() => TextBlockOthersNumber.Text = othersService.OthersFilesCount().ToString()));
            }).Start();

            Canvas.SetZIndex(ListViewMovies, 0);
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

        }

        private void DeleteMovie_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    internal class ListMoviesItem
    {
        public int MovieId { get; set; }
        public BitmapImage MovieCover { get; set; }
    }
}
