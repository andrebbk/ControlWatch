using ControlWatch.Commons.Services;
using ControlWatch.Data;
using ControlWatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Services
{
    public class MovieService : IMovieService
    {
        public List<Movie> GetMovies()
        {
            Console.WriteLine("MovieService.GetMovies: ENTER");
            List<Movie> output = new List<Movie>();

            try
            {
                using (var db = new NorthwindContext())
                {
                    //non-lambda, query-syntax LINQ
                    /*var query = (from o in db.OtherPhotos
                                 where o.IsActive == true
                                 orderby o.ViewsCount ascending, o.Id ascending
                                 select new
                                 {
                                     o.Id,
                                     o.FileName,
                                     o.ViewsCount
                                 }).Skip(skp).Take(tk).ToList(); */

                    //var query = 

                    //if (query.Any())
                    //{
                    //    List<OtherPhoto> output = new List<OtherPhoto>();

                    //    foreach (var item in query)
                    //        output.Add(new OtherPhoto() { Id = item.Id, FileName = item.FileName });

                    //    return output;
                    //}
                    for(int i = 1; i<201; i++)
                    {
                        output.Add(new Movie() { MovieId = i });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting movies -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("MovieService.GetMovies: EXIT");
            return output;
        }
    }
}
