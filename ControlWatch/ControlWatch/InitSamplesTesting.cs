using ControlWatch.Services;
using System;
using System.Configuration;
using System.IO;

namespace ControlWatch
{
    public class InitSamplesTesting
    {
        //TO EXECUTE -> CHANGE TestingMode AppConfig VALUE TO 'true'
        public void InsertSamplesToDB()
        {
            if (Boolean.Parse(ConfigurationManager.AppSettings["TestingMode"]))
            {
                MovieService movieService = new MovieService();
                DirectoryInfo d = new DirectoryInfo(@"C:\Users\andre\Downloads\Proj -ControlWatch\Covers examples");
                int nMovie = 1;
                int movieRating = 1;

                foreach (var file in d.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        string movieTitle = "Movie " + nMovie.ToString();
                        int movieYear = 2000 + movieRating;

                        movieService.CreateMovie(movieTitle, movieYear, false, file.FullName, movieRating, "");

                        if (movieRating == 10)
                            movieRating = 1;
                        else
                            movieRating++;

                        nMovie++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }            
        }
    }
}
