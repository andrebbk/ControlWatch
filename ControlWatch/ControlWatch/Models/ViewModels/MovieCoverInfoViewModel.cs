using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Models.ViewModels
{
    public class MovieCoverInfoViewModel
    {
        public int MovieId { get; set; }

        public string MovieTitle { get; set; }

        public int MovieCoverId { get; set; }

        public string CoverName { get; set; }

        public string CoverPath { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Deleted { get; set; }
    }
}
