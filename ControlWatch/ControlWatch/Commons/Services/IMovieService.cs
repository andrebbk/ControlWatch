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
        IEnumerable<MoviesViewModel> GetMovies(int skp, int tk, string searchTitle, int? searchYear, bool searchFavorite, int? searchRating);

        MovieInfoViewModel GetMovieById(int movieId);

        IEnumerable<MovieInfoViewModel> GetAllMovies(int skp, int tk);

        bool MovieAlreadyExists(string movieTitle, int movieYear, int? movieId = null);

        OutputTypeValues CreateMovie(string movieTitle, int movieYear, bool isFavorite, string movieCover, int ratingValue, string observations);

        OutputTypeValues DeleteMovieById(int movieId);

        OutputTypeValues AddMovieViewById(int movieId);

        string GetMovieTitleById(int movieId);

        OutputTypeValues EditMovie(int movieId, string movieTitle, int movieYear, bool isFavorite, string movieCover, int ratingValue, int movieViews, string observations);

        int? GetMoviesCount(string searchTitle, int? searchYear, bool searchFavorite, int? searchRating, bool allMoviesFlag = false);

        Tuple<int, int> GetAllMoviesCount();

        //Covers services
        Tuple<int, int> GetAllMovieCoversCount();
    }
}
