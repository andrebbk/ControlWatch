using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Models.ViewModels
{
    public class MovieInfoViewModel
    {
        //Movie Info
        public int MovieId { get; set; }

        public string MovieTitle { get; set; }

        public int MovieYear { get; set; }

        public int NrViews { get; set; }

        public bool IsFavorite { get; set; }

        public int MovieRating { get; set; }

        public DateTime CreateDate { get; set; }


        //Cover
        public string CoverName { get; set; }

        public string CoverPath { get; set; }
    }
}
