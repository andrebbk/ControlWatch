using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int moviesCount { get; set; }
        public string MoviesCount
        {
            get { return moviesCount.ToString(); }
        }

        public int moviesViewsCount { get; set; }
        public string MoviesViewsCount
        {
            get { return moviesViewsCount.ToString(); }
        }

        public int tvShowsCount { get; set; }
        public string TvShowsCount
        {
            get { return tvShowsCount.ToString(); }
        }

        public int tvShowsViewsCount { get; set; }
        public string TvShowsViewsCount
        {
            get { return tvShowsViewsCount.ToString(); }
        }
    }
}
