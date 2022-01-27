using ControlWatch.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Data
{
    class NorthwindContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }

        public DbSet<MovieCover> MovieCovers { get; set; }

        public DbSet<TvShow> TvShows { get; set; }

        public DbSet<TvShowCover> TvShowCovers { get; set; }
    }
}
