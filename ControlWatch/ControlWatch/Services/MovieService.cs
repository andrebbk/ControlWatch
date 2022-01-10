using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Commons.Services;
using ControlWatch.Data;
using ControlWatch.Models;
using ControlWatch.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Services
{
    public class MovieService : IMovieService
    {
        public List<MoviesViewModel> GetMovies()
        {
            Console.WriteLine("MovieService.GetMovies: ENTER");
            List<MoviesViewModel> output = new List<MoviesViewModel>();

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
                        output.Add(new MoviesViewModel() { MovieId = i });
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

        public bool MovieAlreadyExists(string movieTitle, int movieYear)
        {
            if (String.IsNullOrWhiteSpace(movieTitle) || movieYear < 1980)
                return true;

            try
            {
                using (var db = new NorthwindContext())
                {
                    return db.Movies.Any(m => m.MovieTitle == movieTitle && m.MovieYear == movieYear && !m.Deleted);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking for existing movie -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return true;
            }
        }

        public OutputTypeValues CreateMovie(string movieTitle, int movieYear, bool isFavorite, string movieCover)
        {
            if (String.IsNullOrWhiteSpace(movieTitle) || movieYear < 1980 || String.IsNullOrWhiteSpace(movieCover))
                return OutputTypeValues.DataError;

            if(MovieAlreadyExists(movieTitle, movieYear))
                return OutputTypeValues.AlreadyExists;

            //save cover first, if it runs successfully, then save data to db
            var newCoverPath = SaveNewMovieCover(movieTitle, movieCover);
            if(String.IsNullOrWhiteSpace(newCoverPath.Item1))
                return OutputTypeValues.SavingCoverError;

            try
            {
                using (var db = new NorthwindContext())
                {
                    Movie movie = new Movie()
                    {
                        MovieTitle = movieTitle,
                        MovieYear = movieYear,
                        NrViews = 1,
                        IsFavorite = isFavorite,
                        CreateDate = DateTime.UtcNow,
                        Deleted = false,
                    };

                    db.Movies.Add(movie);
                    db.SaveChanges();

                    //Save cover
                    var coverResult = CreateCover(movie.MovieId, newCoverPath.Item2, newCoverPath.Item1, db);

                    if (coverResult == OutputTypeValues.Ok)
                        return OutputTypeValues.Ok;
                    else
                        return coverResult;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating new movie -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }


        //Hidden methods
        private Tuple<string, string> SaveNewMovieCover(string newName, string filePath)
        {
            try
            {   //Verify existence and get directory
                string coversFolder = Utils.GetControlWatchMoviesFolder();

                if (!String.IsNullOrEmpty(coversFolder))
                {
                    //get file extension
                    string[] parts = filePath.Split('.'); 

                    // new file name
                    string newFileName = Utils.ClearFormatForString(newName) + "_" + Utils.GetUniqueString(10) + "." + parts[parts.Count() - 1];

                    //new file location
                    string newCoverPath = coversFolder + newFileName;                    

                    //Move file to system respo
                    if (!String.IsNullOrEmpty(newCoverPath))
                        File.Move(filePath, newCoverPath);

                    return Tuple.Create(newCoverPath, newFileName);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR Save new movie cover ->  " + ex.Message);
                return null;
            }

            return null;
        }

        private OutputTypeValues CreateCover(int movieId, string movieCover, string movieCoverPath, NorthwindContext db)
        {
            if (movieId < 0 || String.IsNullOrWhiteSpace(movieCover))
                return OutputTypeValues.DataError;

            try
            {
                if (db.MovieCovers.Any(c => c.MovieId == movieId))
                    return OutputTypeValues.AlreadyExistsCover;

                MovieCover cover = new MovieCover()
                {
                    MovieId = movieId,
                    CoverName = movieCover,
                    CoverPath = movieCoverPath,
                    CreateDate = DateTime.UtcNow,
                    Deleted = false
                };

                db.MovieCovers.Add(cover);
                db.SaveChanges();

                return OutputTypeValues.Ok;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating new movie cover -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }
    }
}
