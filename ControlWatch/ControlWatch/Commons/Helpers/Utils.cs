using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ControlWatch.Commons.Helpers
{
    public static class Utils
    {
        //load image to bitmap without lock file (seek ini stream with 0)
        public static BitmapImage LoadImageToBitmapStreamImage(string fileName)
        {
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
    }
}
