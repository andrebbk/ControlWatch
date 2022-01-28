using ControlWatch.Commons.Enums;
using ControlWatch.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Commons.Services
{
    public interface ITvShowService
    {
        IEnumerable<TvShowsViewModel> GetTvShows(int skp, int tk, string searchTitle, int? searchYear, bool searchFavorite, int? searchRating);

        TvShowInfoViewModel GetTvShowById(int tvShowId);

        bool TvShowAlreadyExists(string tvShowTitle, int tvShowYear, int? tvShowId = null);

        OutputTypeValues CreateTvShow(string tvShowTitle, int tvShowYear, int tvShowSeasons, int tvShowEpisodes, bool isFavorite, string tvShowCover, int ratingValue, string observations);

        OutputTypeValues DeleteTvShowById(int tvShowId);

        OutputTypeValues AddTvShowViewById(int tvShowId);

        string GetTvShowTitleById(int tvShowId);

        OutputTypeValues EditTvShow(int tvShowId, string tvShowTitle, int tvShowYear, int tvShowSeasons, int tvShowEpisodes, bool isFavorite, string tvShowCover, int ratingValue, int tvShowViews, string observations);

        int? GetTvShowsCount(string searchTitle, int? searchYear, bool searchFavorite, int? searchRating);
    }
}
