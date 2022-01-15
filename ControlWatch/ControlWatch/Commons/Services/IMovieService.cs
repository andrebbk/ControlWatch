using ControlWatch.Commons.Enums;
using ControlWatch.Models;
using ControlWatch.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Commons.Services
{
    public interface IMovieService
    {
        IEnumerable<MoviesViewModel> GetMovies(string searchTitle, int? searchYear, bool searchFavorite, int? searchRating);

        MovieInfoViewModel GetMovieById(int movieId);

        bool MovieAlreadyExists(string movieTitle, int movieYear);

        OutputTypeValues CreateMovie(string movieTitle, int movieYear, bool isFavorite, string movieCover, int ratingValue);

        OutputTypeValues DeleteMovieById(int movieId);

        OutputTypeValues AddMovieViewById(int movieId);

        string GetMovieTitleById(int movieId);
    }
}
