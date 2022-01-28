using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Popup;
using ControlWatch.Windows.Dashboard;
using ControlWatch.Windows.Movies;
using ControlWatch.Windows.SideMenu;
using ControlWatch.Windows.TvShows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ControlWatch
{   
    public partial class MainWindow : Window
    {
        private bool IsDrawableOpen;
        private MenuOptionsTypeValues activeMenuOption;

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;

            InitializeComponent();

            //Mostrar a barra de tarefas do windows
            this.MaxHeight = SystemParameters.WorkArea.Height + 14;

            LoadMainWindow();
        }

        private void LoadMainWindow()
        {
            (new InitSamplesTesting()).InsertSamplesToDB();

            IsDrawableOpen = false;
            this.DrawableMenuContainer.Content = new SideMenu_UserControl(this);

            //Init content
            this.MainContainer.Content = new Dashboard_UserControl(this);
            activeMenuOption = MenuOptionsTypeValues.Dashboard;

            //Only for testing - REMOVE THIS NEXT LINES
            this.MainContainer.Content = new NewMovie_UserControl(this);
            activeMenuOption = MenuOptionsTypeValues.NewMovie;

        }

        //Open Drawable Menu
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            IsDrawableOpen = true;
            ButtonOpen.Visibility = Visibility.Hidden;
            Canvas.SetZIndex(this.GridBackground, 0);
        }

        //Click on background
        private void GridBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            if (IsDrawableOpen)
            {
                Storyboard sb = this.FindResource("CloseMenu") as Storyboard;
                sb.Begin();

                IsDrawableOpen = false;

                ButtonOpen.Visibility = Visibility.Visible;

                Canvas.SetZIndex(this.GridBackground, -2);
            }
        }

        //Binding Actions    
        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }


        //Public Acess
        public void SetMainContent(MenuOptionsTypeValues menuOption, int? movieId = null, int? tvShowId = null)
        {
            switch (menuOption)
            {
                case MenuOptionsTypeValues.Dashboard:
                    if(activeMenuOption != MenuOptionsTypeValues.Dashboard)
                    {
                        this.MainContainer.Content = new Dashboard_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.Dashboard;
                    }                    
                    break;
                case MenuOptionsTypeValues.Movies:
                    if (activeMenuOption != MenuOptionsTypeValues.Movies)
                    {
                        this.MainContainer.Content = new Movies_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.Movies;
                    }                        
                    break;
                case MenuOptionsTypeValues.NewMovie:
                    if (activeMenuOption != MenuOptionsTypeValues.NewMovie)
                    {
                        this.MainContainer.Content = new NewMovie_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.NewMovie;
                    }
                    break;
                case MenuOptionsTypeValues.TvShows:
                    if (activeMenuOption != MenuOptionsTypeValues.TvShows)
                    {
                        this.MainContainer.Content = new TvShows_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.TvShows;
                    }
                    break;
                case MenuOptionsTypeValues.NewTvShow:
                    if (activeMenuOption != MenuOptionsTypeValues.NewTvShow)
                    {
                        this.MainContainer.Content = new NewTvShow_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.NewTvShow;
                    }
                    break;
                case MenuOptionsTypeValues.MovieInfo:
                    if (activeMenuOption != MenuOptionsTypeValues.MovieInfo)
                    {
                        this.MainContainer.Content = new MovieInfo_UserControl(this, movieId);
                        activeMenuOption = MenuOptionsTypeValues.MovieInfo;
                    }
                    break;
                case MenuOptionsTypeValues.TvShowInfo:
                    if (activeMenuOption != MenuOptionsTypeValues.TvShowInfo)
                    {
                        this.MainContainer.Content = new TvShowInfo_UserControl(this, tvShowId);
                        activeMenuOption = MenuOptionsTypeValues.TvShowInfo;
                    }
                    break;
                default:
                    break;
            }
        }

        public void CloseSideDrawableMenu()
        {
            Storyboard sb = this.FindResource("CloseMenu") as Storyboard;
            sb.Begin();

            IsDrawableOpen = false;

            ButtonOpen.Visibility = Visibility.Visible;

            Canvas.SetZIndex(this.GridBackground, -2);
        }
    }
}
