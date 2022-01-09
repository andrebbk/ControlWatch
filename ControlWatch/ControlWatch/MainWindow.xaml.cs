using ControlWatch.Commons.Enums;
using ControlWatch.Windows.Dashboard;
using ControlWatch.Windows.Movies;
using ControlWatch.Windows.SideMenu;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            IsDrawableOpen = false;
            this.DrawableMenuContainer.Content = new SideMenu_UserControl(this);

            //Init content
            this.MainContainer.Content = new Dashboard_UserControl(this);
            activeMenuOption = MenuOptionsTypeValues.Dashboard;
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
        public void SetMainContent(MenuOptionsTypeValues menuOption)
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
                        //this.MainContainer.Content = new Movies_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.NewMovie;
                    }
                    break;
                case MenuOptionsTypeValues.TvShows:
                    if (activeMenuOption != MenuOptionsTypeValues.TvShows)
                    {
                        //this.MainContainer.Content = new Movies_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.TvShows;
                    }
                    break;
                case MenuOptionsTypeValues.NewTvShow:
                    if (activeMenuOption != MenuOptionsTypeValues.NewTvShow)
                    {
                        //this.MainContainer.Content = new Movies_UserControl(this);
                        activeMenuOption = MenuOptionsTypeValues.NewTvShow;
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
