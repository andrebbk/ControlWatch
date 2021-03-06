using ControlWatch.Commons.Enums;
using ControlWatch.Notifications.CustomMessage;
using ControlWatch.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ControlWatch.Commons.Helpers
{
    public static class Utils
    {        

        //get folder directory for movie's cover
        public static string GetControlWatchMoviesFolder()
        {
            var serviceConfig = new ConfigurationService();
            string output = null;

            //var exePath = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            //if (!String.IsNullOrEmpty(exePath.ToString()))
            //{
            //    string coversFolder = exePath.ToString() + "\\Covers\\Movies\\";

            //    if (!Directory.Exists(coversFolder))
            //        Directory.CreateDirectory(coversFolder);

            //    output = coversFolder;
            //}

            var moviesCoverPath = serviceConfig.GetCurrentCoverPath(EntityTypeValues.Movie);
            if(!String.IsNullOrEmpty(moviesCoverPath) && Directory.Exists(moviesCoverPath))
            {
                output = moviesCoverPath;
            }
            else
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Movies cover path is invalid!");
            }

            return output;
        }

        //get folder directory for tvshow's cover
        public static string GetControlWatchTvShowsFolder()
        {
            var serviceConfig = new ConfigurationService();
            string output = null;

            //var exePath = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            //if (!String.IsNullOrEmpty(exePath.ToString()))
            //{
            //    string coversFolder = exePath.ToString() + "\\Covers\\TvShows\\";

            //    if (!Directory.Exists(coversFolder))
            //        Directory.CreateDirectory(coversFolder);

            //    output = coversFolder;
            //}

            var tvShowsCoverPath = serviceConfig.GetCurrentCoverPath(EntityTypeValues.TvShow);
            if (!String.IsNullOrEmpty(tvShowsCoverPath) && Directory.Exists(tvShowsCoverPath))
            {
                output = tvShowsCoverPath;
            }
            else
            {
                NotificationHelper.notifier.ShowCustomMessage("Control Watch", "Tv shows cover path is invalid!");
            }

            return output;
        }

        //load image to bitmap without lock file (seek ini stream with 0)
        public static BitmapImage LoadImageToBitmapStreamImage(string fileName)
        {
            if (!File.Exists(fileName)) return null;

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapImage thumb = new BitmapImage();
                fs.Seek(0, SeekOrigin.Begin);
                thumb.BeginInit();
                thumb.CreateOptions =
                    BitmapCreateOptions.PreservePixelFormat |
                    BitmapCreateOptions.IgnoreColorProfile;
                thumb.CacheOption =
                    BitmapCacheOption.OnLoad;
                thumb.DecodePixelWidth = 200;
                thumb.StreamSource = fs;
                thumb.EndInit();
                thumb.Freeze();
                return thumb;
            }
        }

        //load image to bitmap without lock file and no decode resolution
        public static BitmapImage LoadImageToBitmapImageNoDecodeChange(string fileName)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        //load image to bitmap from resources
        public static BitmapImage LoadImageToBitmapFromResources(string filepath)
        {
            return new BitmapImage(new Uri(@filepath, UriKind.Relative));       
        }

        //Clear string content
        public static string ClearFormatForString(string textString)
        {
            return Regex.Replace(textString, "[^a-zA-Z]", "").ToLower();
        }

        //Random AlphaNumeric string withh RNGCrypto
        public static string GetUniqueString(int string_length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bit_count = (string_length * 6);
                var byte_count = ((bit_count + 7) / 8); // rounded up
                var bytes = new byte[byte_count];
                rng.GetBytes(bytes);

                //Only Numeric - letter
                return Regex.Replace(Convert.ToBase64String(bytes), @"\W+", "");
            }
        }
    }
}
