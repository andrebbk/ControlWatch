using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Models.ViewModels
{
    public class TvShowInfoViewModel
    {
        //Movie Info
        public int TvShowId { get; set; }

        public string TvShowTitle { get; set; }

        public int TvShowYear { get; set; }

        public int TvShowSeasons { get; set; }

        public int TvShowEpisodes { get; set; }

        public int NrViews { get; set; }

        public bool IsFavorite { get; set; }

        public int TvShowRating { get; set; }

        public string Observations { get; set; }

        public DateTime CreateDate { get; set; }


        //Cover
        public string CoverName { get; set; }

        public string CoverPath { get; set; }
    }
}
