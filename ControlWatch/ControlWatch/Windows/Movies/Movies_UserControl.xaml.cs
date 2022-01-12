using ControlWatch.Commons.Helpers;
using ControlWatch.Models.ViewModels;
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

namespace ControlWatch.Windows.Movies
{
    /// <summary>
    /// Interaction logic for Movies_UserControl.xaml
    /// </summary>
    public partial class Movies_UserControl : UserControl
    {
        private MainWindow _mainWindow;

        private MovieService moviesService;

        public Movies_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;

            this.moviesService = new MovieService();

            LoadMovieList();
        }

        private void LoadMovieList()
        {
            new Thread(() =>
            {
                var moviesList = moviesService.GetMovies();              

                if (moviesList != null)
                {
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = null));
                    ObservableCollection<MoviesViewModel> moviesToShow = new ObservableCollection<MoviesViewModel>();

                    foreach (var item in moviesList)
                    {
                        //load imagem
                        if(!String.IsNullOrEmpty(item.MovieCoverPath))
                            item.MovieCover = Utils.LoadImageToBitmapStreamImage(item.MovieCoverPath);

                        moviesToShow.Add(item);
                    }                    

                    //BINDING
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ItemsSource = moviesToShow));

                    //RESTART SCROLLBAR
                    ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.ScrollIntoView(ListViewMovies.Items[0])));
                }

                //NUMBER
                //TextBlockOthersNumber.Dispatcher.BeginInvoke((Action)(() => TextBlockOthersNumber.Text = othersService.OthersFilesCount().ToString()));

                //ListViewMovies.Dispatcher.BeginInvoke((Action)(() => ListViewMovies.IsEnabled = true));
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

        private void Button_Pag_Left_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Pag_First_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Pag_Last_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Pag_Right_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
