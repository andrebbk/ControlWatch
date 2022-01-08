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

        public MainWindow()
        {            
            InitializeComponent();
            LoadMainWindow();
        }

        private void LoadMainWindow()
        {
            IsDrawableOpen = true;
            this.DrawableMenuContainer.Content = new SideMenu_UserControl(this);
        }

        //Open Drawable Menu
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            IsDrawableOpen = true;
            ButtonOpen.Visibility = Visibility.Hidden;
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
            }
        }

        //Binding Actions    
        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
