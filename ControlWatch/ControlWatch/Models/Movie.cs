using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        public string MovieTitle { get; set; }

        public int MovieYear { get; set; }

        public int NrViews { get; set; }

        public bool IsFavorite { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Deleted { get; set; }


        public MovieCover movieCover { get; set; }
    }
}
