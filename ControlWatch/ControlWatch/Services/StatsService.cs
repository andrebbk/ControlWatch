using ControlWatch.Commons.Services;
using ControlWatch.Data;
using ControlWatch.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Services
{
    public class StatsService : IStatsService
    {
        public DashboardViewModel GetStats()
        {
            Console.WriteLine("StatsService.GetStats: ENTER");
            DashboardViewModel output = new DashboardViewModel();

            try
            {
                using (var db = new NorthwindContext())
                {
                    //Movies
                    output.moviesCount = db.Movies.Where(m => !m.Deleted).Count();
                    output.moviesViewsCount = db.Movies.Where(m => !m.Deleted).Sum(m => m.NrViews);

                    //TvShows
                    output.tvShowsCount = 0; //db.Movies.Where(m => !m.Deleted).Count();
                    output.tvShowsViewsCount = 0;//db.Movies.Where(m => !m.Deleted).Sum(m => m.NrViews);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting stats to dashboard -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("StatsService.GetStats: EXIT");
            return output;
        }
    }
}
