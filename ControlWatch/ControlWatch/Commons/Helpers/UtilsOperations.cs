using ControlWatch.Popup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ControlWatch.Commons.Helpers
{
    public static class UtilsOperations
    {
        public static LoadingWindow loadingWindow;

        //Loading Animation
        public static LottieSharp.LottieAnimationView LoadingLottieAnimationView { get; set; }

        public static void StartLoadingAnimation()
        {
            new Thread(() =>
            {        
                Application.Current.Dispatcher.Invoke(
                                      new Action(() => {
                       loadingWindow = new LoadingWindow();
                       loadingWindow.LottieAnimationView.PlayAnimation();
                       loadingWindow.ShowDialog();
                   }));

            }).Start();
        }

        public static void StopLoadingAnimation()
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);                

                loadingWindow.Dispatcher.BeginInvoke((Action)(() => loadingWindow.Close()));

                //Check for other windows opened
                Application.Current.Dispatcher.Invoke(
                    new Action(() => {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if(window.GetType() == typeof(LoadingWindow) || window.GetType() == typeof(ConfirmWindow))
                            {
                                window.Close();
                            }
                        }
                    }));                

            }).Start();
        }
    }
}
