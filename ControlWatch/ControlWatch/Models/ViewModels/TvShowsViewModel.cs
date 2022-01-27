using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ControlWatch.Models.ViewModels
{
    public class TvShowsViewModel
    {
        public int TvShowId { get; set; }

        public BitmapImage TvShowCover { get; set; }

        public string TvShowCoverPath { get; set; }
    }
}
