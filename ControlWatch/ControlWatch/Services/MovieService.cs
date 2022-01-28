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
        public IEnumerable<MoviesViewModel> GetMovies(int skp, int tk, string searchTitle, int? searchYear, bool searchFavorite, int? searchRating)
        {
            Console.WriteLine("MovieService.GetMovies: ENTER");
            List<MoviesViewModel> output = new List<MoviesViewModel>();

            try
            {
                using (var db = new NorthwindContext())
                {
                    var query = (from m in db.Movies
                                 join c in db.MovieCovers on m.MovieId equals c.MovieId
                                 where !m.Deleted
                                 orderby m.MovieYear descending, m.MovieTitle ascending
                                 select new 
                                 {
                                     m.MovieId,
                                     m.MovieTitle,
                                     m.MovieYear,
                                     m.IsFavorite,
                                     m.MovieRating,
                                     c.CoverPath
                                 })
                                 .Skip(skp)
                                 .Take(tk);

                    //Apply filters
                    if (!String.IsNullOrEmpty(searchTitle))
                    {
                        query = query.Where(m => m.MovieTitle.ToLower().Contains(searchTitle.ToLower().Trim()));
                    }
                    if (searchYear.HasValue && searchYear.Value > 1979)
                    {
                        query = query.Where(m => m.MovieYear == searchYear.Value);
                    }
                    if (searchFavorite)
                    {
                        query = query.Where(m => m.IsFavorite);
                    }
                    if (searchRating.HasValue)
                    {
                        query = query.Where(m => m.MovieRating == searchRating.Value);
                    }

                    if (query.Any())
                    {
                        output = query.Select(m => new MoviesViewModel
                        {
                            MovieId = m.MovieId,
                            MovieCoverPath = m.CoverPath
                        }).ToList();
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

        public MovieInfoViewModel GetMovieById(int movieId)
        {
            Console.WriteLine("MovieService.GetMovieById: ENTER");
            MovieInfoViewModel output = null;

            try
            {
                using (var db = new NorthwindContext())
                {
                    var query = (from m in db.Movies
                                 join c in db.MovieCovers on m.MovieId equals c.MovieId
                                 where !m.Deleted && !c.Deleted
                                 && m.MovieId == movieId
                                 select new
                                 {
                                     m.MovieId,
                                     m.MovieTitle,
                                     m.MovieYear,
                                     m.NrViews,
                                     m.IsFavorite,
                                     m.MovieRating,
                                     m.Observations,
                                     m.CreateDate,
                                     c.CoverName,
                                     c.CoverPath
                                 });                    

                    if (query != null && query.Any())
                    {
                        output = query.Select(m => new MovieInfoViewModel
                        {
                            MovieId = m.MovieId,
                            MovieTitle = m.MovieTitle,
                            MovieYear = m.MovieYear,
                            NrViews = m.NrViews,
                            IsFavorite = m.IsFavorite,
                            MovieRating = m.MovieRating,
                            Observations = m.Observations,
                            CreateDate = m.CreateDate,
                            CoverName = m.CoverName,
                            CoverPath = m.CoverPath
                        }).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error getting movie with id {0} -> ", movieId.ToString()) + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("MovieService.GetMovieById: EXIT");
            return output;
        }

        public bool MovieAlreadyExists(string movieTitle, int movieYear, int? movieId = null)
        {
            Console.WriteLine("MovieService.MovieAlreadyExists: ENTER");
            if (String.IsNullOrWhiteSpace(movieTitle) || movieYear < 1980)
                return true;

            try
            {
                using (var db = new NorthwindContext())
                {
                    if(movieId.HasValue)
                        return db.Movies.Any(m => m.MovieId != movieId.Value && m.MovieTitle == movieTitle && m.MovieYear == movieYear && !m.Deleted);
                    else
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

        public OutputTypeValues CreateMovie(string movieTitle, int movieYear, bool isFavorite, string movieCover, int ratingValue, string observations)
        {
            Console.WriteLine("MovieService.CreateMovie: ENTER");
            if (String.IsNullOrWhiteSpace(movieTitle) || movieYear < 1980 || String.IsNullOrWhiteSpace(movieCover))
                return OutputTypeValues.DataError;

            if(MovieAlreadyExists(movieTitle, movieYear))
                return OutputTypeValues.AlreadyExists;            

            try
            {
                //save cover first, if it runs successfully, then save data to db
                var newCoverPath = SaveNewMovieCover(movieTitle, movieCover);
                if (String.IsNullOrWhiteSpace(newCoverPath.Item1))
                    return OutputTypeValues.SavingCoverError;

                using (var db = new NorthwindContext())
                {
                    Movie movie = new Movie()
                    {
                        MovieTitle = movieTitle,
                        MovieYear = movieYear,
                        NrViews = 1,
                        IsFavorite = isFavorite,
                        MovieRating = ratingValue,
                        Observations = observations,
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

        public OutputTypeValues DeleteMovieById(int movieId)
        {
            Console.WriteLine("MovieService.DeleteMovieById: ENTER");
            try
            {
                using (var db = new NorthwindContext())
                {
                    var movie = db.Movies.Where(m => m.MovieId == movieId && !m.Deleted).FirstOrDefault();
                    if (movie != null)
                    {
                        //Delete cover
                        var movieCover = db.MovieCovers.Where(c => c.MovieId == movieId && !c.Deleted).FirstOrDefault();
                        if(movieCover != null)
                        {
                            if (!String.IsNullOrEmpty(movieCover.CoverPath))
                            {
                                if (File.Exists(movieCover.CoverPath))
                                    File.Delete(movieCover.CoverPath);

                                movieCover.Deleted = true;
                                db.SaveChanges();
                            }
                        }

                        //Delete Movie
                        movie.Deleted = true;
                        db.SaveChanges();

                        Console.WriteLine("MovieService.DeleteMovieById: EXIT");
                        return OutputTypeValues.Ok;
                    }
                }

                Console.WriteLine("MovieService.DeleteMovieById: EXIT");
                return OutputTypeValues.Error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting movie -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        public OutputTypeValues AddMovieViewById(int movieId)
        {
            Console.WriteLine("MovieService.AddMovieViewById: ENTER");
            try
            {
                using (var db = new NorthwindContext())
                {
                    var movie = db.Movies.Where(m => m.MovieId == movieId && !m.Deleted).FirstOrDefault();
                    if (movie != null)
                    {
                        //update Movie visualizations
                        movie.NrViews = movie.NrViews + 1;
                        db.SaveChanges();

                        return OutputTypeValues.Ok;
                    }
                }

                return OutputTypeValues.Error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating movie views -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        public string GetMovieTitleById(int movieId)
        {
            Console.WriteLine("MovieService.GetMovieTitleById: ENTER");
            string output = null;
            try
            {
                using (var db = new NorthwindContext())
                {
                    var movie = db.Movies.Where(m => m.MovieId == movieId && !m.Deleted).FirstOrDefault();
                    if (movie != null)
                    {
                        output = movie.MovieTitle;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at MovieService.GetMovieTitleById -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("MovieService.GetMovieTitleById: END");
            return output;
        }

        public OutputTypeValues EditMovie(int movieId, string movieTitle, int movieYear, bool isFavorite, string movieCover, int ratingValue, int movieViews, string observations)
        {
            Console.WriteLine("MovieService.EditMovie: ENTER");
            if (String.IsNullOrWhiteSpace(movieTitle) || movieYear < 1980 || movieViews < 1)
                return OutputTypeValues.DataError;

            if (MovieAlreadyExists(movieTitle, movieYear, movieId))
                return OutputTypeValues.AlreadyExists;                  

            try
            {
                using (var db = new NorthwindContext())
                {
                    var movie = db.Movies.Where(m => m.MovieId == movieId && !m.Deleted).FirstOrDefault();
                    if(movie != null)
                    {
                        //edit cover first, if it runs successfully, then save data to db
                        Tuple<string, string> newCoverPath = null;
                        if (!String.IsNullOrEmpty(movieCover))
                        {
                            newCoverPath = SaveNewMovieCover(movieTitle, movieCover);
                            if (newCoverPath is null || (newCoverPath != null && String.IsNullOrWhiteSpace(newCoverPath.Item1)))
                                return OutputTypeValues.SavingCoverError;
                        }

                        //Update movie
                        movie.MovieTitle = movieTitle;
                        movie.MovieYear = movieYear;
                        movie.NrViews = movieViews;
                        movie.IsFavorite = isFavorite;
                        movie.MovieRating = ratingValue;
                        movie.Observations = observations;
                        db.SaveChanges();

                        //Save new cover
                        if(newCoverPath != null)
                        {
                            var coverResult = EditCover(movie.MovieId, newCoverPath.Item2, newCoverPath.Item1, db);

                            if (coverResult == OutputTypeValues.Ok)
                                return OutputTypeValues.Ok;
                            else
                                return coverResult;
                        }

                        return OutputTypeValues.Ok;

                    }

                    return OutputTypeValues.MovieNotFound;                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error editing movie -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        public int? GetMoviesCount(string searchTitle, int? searchYear, bool searchFavorite, int? searchRating)
        {
            Console.WriteLine("MovieService.GetMoviesCount: ENTER");

            try
            {
                using (var db = new NorthwindContext())
                {
                    var query = (from m in db.Movies
                                 where !m.Deleted
                                 select new
                                 {
                                     m.MovieTitle,
                                     m.MovieYear,
                                     m.IsFavorite,
                                     m.MovieRating
                                 });

                    //Apply filters
                    if (!String.IsNullOrEmpty(searchTitle))
                    {
                        query = query.Where(m => m.MovieTitle.ToLower().Contains(searchTitle.ToLower().Trim()));
                    }
                    if (searchYear.HasValue && searchYear.Value > 1979)
                    {
                        query = query.Where(m => m.MovieYear == searchYear.Value);
                    }
                    if (searchFavorite)
                    {
                        query = query.Where(m => m.IsFavorite);
                    }
                    if (searchRating.HasValue)
                    {
                        query = query.Where(m => m.MovieRating == searchRating.Value);
                    }

                    Console.WriteLine("MovieService.GetMoviesCount: EXIT");
                    return query.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting movies count -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("MovieService.GetMoviesCount: EXIT");
            return null;
        }


        //Hidden methods
        private Tuple<string, string> SaveNewMovieCover(string newName, string filePath)
        {
            Console.WriteLine("MovieService.SaveNewMovieCover: ENTER");

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

                    Console.WriteLine("MovieService.SaveNewMovieCover: EXIT");
                    return Tuple.Create(newCoverPath, newFileName);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR Save new movie cover ->  " + ex.Message);
                return null;
            }

            Console.WriteLine("MovieService.SaveNewMovieCover: EXIT");
            return null;
        }

        private OutputTypeValues CreateCover(int movieId, string movieCover, string movieCoverPath, NorthwindContext db)
        {
            Console.WriteLine("MovieService.CreateCover: ENTER");
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

        private OutputTypeValues EditCover(int movieId, string movieCover, string movieCoverPath, NorthwindContext db)
        {
            Console.WriteLine("MovieService.EditCover: ENTER");
            if (movieId < 0 || String.IsNullOrWhiteSpace(movieCover))
                return OutputTypeValues.DataError;

            try
            {
                var existingCover = db.MovieCovers.Where(c => c.MovieId == movieId && !c.Deleted).FirstOrDefault();
                if(existingCover != null)
                {
                    //Remove old cover
                    if (!String.IsNullOrEmpty(existingCover.CoverPath) && File.Exists(existingCover.CoverPath))
                        File.Delete(existingCover.CoverPath);

                    //Update movie cover
                    existingCover.CoverName = movieCover;
                    existingCover.CoverPath = movieCoverPath;
                    db.SaveChanges();

                    Console.WriteLine("MovieService.EditCover: EXIT");
                    return OutputTypeValues.Ok;
                }

                return OutputTypeValues.MovieCoverNotFound;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error editing movie cover -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }
    }
}
