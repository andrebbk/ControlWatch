using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ControlWatch.Models.ViewModels
{
    public class MoviesViewModel
    {
        public int MovieId { get; set; }        

        public BitmapImage MovieCover { get; set; }

        public string MovieCoverPath { get; set; }
    }
}
