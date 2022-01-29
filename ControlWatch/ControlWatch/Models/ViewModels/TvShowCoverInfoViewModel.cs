using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Models.ViewModels
{
    public class TvShowCoverInfoViewModel
    {
        public int TvShowId { get; set; }

        public string TvShowTitle { get; set; }

        public int TvShowCoverId { get; set; }

        public string CoverName { get; set; }

        public string CoverPath { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Deleted { get; set; }
    }
}
