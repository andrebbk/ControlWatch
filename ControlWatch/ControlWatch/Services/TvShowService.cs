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
    public class TvShowService : ITvShowService
    {
        public IEnumerable<TvShowsViewModel> GetTvShows(int skp, int tk, string searchTitle, int? searchYear, bool searchFavorite, int? searchRating, bool searchFinished)
        {
            Console.WriteLine("TvShowService.GetTvShows: ENTER");
            List<TvShowsViewModel> output = new List<TvShowsViewModel>();

            try
            {
                using (var db = new NorthwindContext())
                {
                    var query = (from t in db.TvShows
                                 join c in db.TvShowCovers on t.TvShowId equals c.TvShowId
                                 where !t.Deleted
                                 orderby t.TvShowYear descending, t.TvShowTitle ascending
                                 select new
                                 {
                                     t.TvShowId,
                                     t.TvShowTitle,
                                     t.TvShowYear,
                                     t.IsFavorite,
                                     t.TvShowRating,
                                     t.IsFinished,
                                     c.CoverPath
                                 })
                                 .Skip(skp)
                                 .Take(tk);

                    //Apply filters
                    if (!String.IsNullOrEmpty(searchTitle))
                    {
                        query = query.Where(t => t.TvShowTitle.ToLower().Contains(searchTitle.ToLower().Trim()));
                    }
                    if (searchYear.HasValue && searchYear.Value > 1979)
                    {
                        query = query.Where(t => t.TvShowYear == searchYear.Value);
                    }
                    if (searchFavorite)
                    {
                        query = query.Where(t => t.IsFavorite);
                    }
                    if (searchRating.HasValue)
                    {
                        query = query.Where(t => t.TvShowRating == searchRating.Value);
                    }
                    if (searchFinished)
                    {
                        query = query.Where(t => t.IsFinished);
                    }

                    if (query.Any())
                    {
                        output = query.Select(t => new TvShowsViewModel
                        {
                            TvShowId = t.TvShowId,
                            TvShowCoverPath = t.CoverPath
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting tvShows -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("TvShowService.GetTvShows: EXIT");
            return output;
        }

        public TvShowInfoViewModel GetTvShowById(int tvShowId)
        {
            Console.WriteLine("TvShowService.GetTvShowById: ENTER");
            TvShowInfoViewModel output = null;

            try
            {
                using (var db = new NorthwindContext())
                {
                    var query = (from t in db.TvShows
                                 join c in db.TvShowCovers on t.TvShowId equals c.TvShowId
                                 where !t.Deleted && !c.Deleted
                                 && t.TvShowId == tvShowId
                                 select new
                                 {
                                     t.TvShowId,
                                     t.TvShowTitle,
                                     t.TvShowYear,
                                     t.TvShowSeasons,
                                     t.TvShowEpisodes,
                                     t.NrViews,
                                     t.IsFavorite,
                                     t.TvShowRating,
                                     t.IsFinished,
                                     t.Observations,
                                     t.CreateDate,
                                     c.CoverName,
                                     c.CoverPath
                                 });

                    if (query != null && query.Any())
                    {
                        output = query.Select(t => new TvShowInfoViewModel
                        {
                            TvShowId = t.TvShowId,
                            TvShowTitle = t.TvShowTitle,
                            TvShowYear = t.TvShowYear,
                            TvShowSeasons = t.TvShowSeasons,
                            TvShowEpisodes = t.TvShowEpisodes,
                            NrViews = t.NrViews,
                            IsFavorite = t.IsFavorite,
                            TvShowRating = t.TvShowRating,
                            IsFinished = t.IsFinished,
                            Observations = t.Observations,
                            CreateDate = t.CreateDate,
                            CoverName = t.CoverName,
                            CoverPath = t.CoverPath
                        }).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error getting tvshow with id {0} -> ", tvShowId.ToString()) + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("TvShowService.GetTvShowById: EXIT");
            return output;
        }

        public bool TvShowAlreadyExists(string tvShowTitle, int tvShowYear, int? tvShowId = null)
        {
            Console.WriteLine("TvShowService.TvShowAlreadyExists: ENTER");
            if (String.IsNullOrWhiteSpace(tvShowTitle) || tvShowYear < 1980)
                return true;

            try
            {
                using (var db = new NorthwindContext())
                {
                    if (tvShowId.HasValue)
                        return db.TvShows.Any(t => t.TvShowId != tvShowId.Value && t.TvShowTitle == tvShowTitle && t.TvShowYear == tvShowYear && !t.Deleted);
                    else
                        return db.TvShows.Any(t => t.TvShowTitle == tvShowTitle && t.TvShowYear == tvShowYear && !t.Deleted);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking for existing tvshow -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return true;
            }
        }

        public OutputTypeValues CreateTvShow(string tvShowTitle, int tvShowYear, int tvShowSeasons, int tvShowEpisodes, bool isFavorite, string tvShowCover, int ratingValue, string observations)
        {
            Console.WriteLine("TvShowService.CreateTvShow: ENTER");
            if (String.IsNullOrWhiteSpace(tvShowTitle) || tvShowYear < 1980 || String.IsNullOrWhiteSpace(tvShowCover))
                return OutputTypeValues.DataError;

            if (TvShowAlreadyExists(tvShowTitle, tvShowYear))
                return OutputTypeValues.AlreadyExists;

            try
            {
                //save cover first, if it runs successfully, then save data to db
                var newCoverPath = SaveNewTvShowCover(tvShowTitle, tvShowCover);
                if (String.IsNullOrWhiteSpace(newCoverPath.Item1))
                    return OutputTypeValues.SavingCoverError;

                using (var db = new NorthwindContext())
                {
                    TvShow tvShow = new TvShow()
                    {
                        TvShowTitle = tvShowTitle,
                        TvShowYear = tvShowYear,
                        NrViews = 1,
                        TvShowSeasons = tvShowSeasons,
                        TvShowEpisodes = tvShowEpisodes,
                        IsFavorite = isFavorite,
                        TvShowRating = ratingValue,
                        IsFinished = false,
                        Observations = observations,
                        CreateDate = DateTime.UtcNow,
                        Deleted = false,
                    };

                    db.TvShows.Add(tvShow);
                    db.SaveChanges();

                    //Save cover
                    var coverResult = CreateCover(tvShow.TvShowId, newCoverPath.Item2, newCoverPath.Item1, db);

                    if (coverResult == OutputTypeValues.Ok)
                        return OutputTypeValues.Ok;
                    else
                        return coverResult;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating new tvshow -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        public OutputTypeValues DeleteTvShowById(int tvShowId)
        {
            Console.WriteLine("TvShowService.DeleteTvShowById: ENTER");
            try
            {
                using (var db = new NorthwindContext())
                {
                    var tvShow = db.TvShows.Where(t => t.TvShowId == tvShowId && !t.Deleted).FirstOrDefault();
                    if (tvShow != null)
                    {
                        //Delete cover
                        var tvShowCover = db.TvShowCovers.Where(c => c.TvShowId == tvShowId && !c.Deleted).FirstOrDefault();
                        if (tvShowCover != null)
                        {
                            if (!String.IsNullOrEmpty(tvShowCover.CoverPath))
                            {
                                if (File.Exists(tvShowCover.CoverPath))
                                    File.Delete(tvShowCover.CoverPath);

                                tvShowCover.Deleted = true;
                                db.SaveChanges();
                            }
                        }

                        //Delete Movie
                        tvShow.Deleted = true;
                        db.SaveChanges();

                        Console.WriteLine("TvShowService.DeleteTvShowById: EXIT");
                        return OutputTypeValues.Ok;
                    }
                }

                Console.WriteLine("TvShowService.DeleteTvShowById: EXIT");
                return OutputTypeValues.Error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting tvshow -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        public OutputTypeValues AddTvShowViewById(int tvShowId)
        {
            Console.WriteLine("TvShowService.AddTvShowViewById: ENTER");
            try
            {
                using (var db = new NorthwindContext())
                {
                    var tvShow = db.TvShows.Where(t => t.TvShowId == tvShowId && !t.Deleted).FirstOrDefault();
                    if (tvShow != null)
                    {
                        //update TvShow visualizations 
                        tvShow.NrViews = tvShow.NrViews + 1;
                        db.SaveChanges();

                        return OutputTypeValues.Ok;
                    }
                }

                return OutputTypeValues.Error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating tvshow" +
                    " views -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        public string GetTvShowTitleById(int tvShowId)
        {
            Console.WriteLine("TvShowService.GetTvShowTitleById: ENTER");
            string output = null;
            try
            {
                using (var db = new NorthwindContext())
                {
                    var tvShow = db.TvShows.Where(t => t.TvShowId == tvShowId && !t.Deleted).FirstOrDefault();
                    if (tvShow != null)
                    {
                        output = tvShow.TvShowTitle;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at TvShowService.GetTvShowTitleById -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("TvShowService.GetTvShowTitleById: END");
            return output;
        }

        public OutputTypeValues EditTvShow(int tvShowId, string tvShowTitle, int tvShowYear, int tvShowSeasons, int tvShowEpisodes, bool isFavorite, string tvShowCover, int ratingValue, int tvShowViews, bool isFinished, string observations)
        {
            Console.WriteLine("TvShowService.EditTvShow: ENTER");
            if (String.IsNullOrWhiteSpace(tvShowTitle) || tvShowYear < 1980 || tvShowViews < 1)
                return OutputTypeValues.DataError;

            if (TvShowAlreadyExists(tvShowTitle, tvShowYear, tvShowId))
                return OutputTypeValues.AlreadyExists;

            try
            {
                using (var db = new NorthwindContext())
                {
                    var tvShow = db.TvShows.Where(t => t.TvShowId == tvShowId && !t.Deleted).FirstOrDefault();
                    if (tvShow != null)
                    {
                        //edit cover first, if it runs successfully, then save data to db
                        Tuple<string, string> newCoverPath = null;
                        if (!String.IsNullOrEmpty(tvShowCover))
                        {
                            newCoverPath = SaveNewTvShowCover(tvShowTitle, tvShowCover);
                            if (newCoverPath is null || (newCoverPath != null && String.IsNullOrWhiteSpace(newCoverPath.Item1)))
                                return OutputTypeValues.SavingCoverError;
                        }

                        //Update tvshow
                        tvShow.TvShowTitle = tvShowTitle;
                        tvShow.TvShowYear = tvShowYear;
                        tvShow.TvShowSeasons = tvShowSeasons;
                        tvShow.TvShowEpisodes = tvShowEpisodes;
                        tvShow.NrViews = tvShowViews;
                        tvShow.IsFavorite = isFavorite;
                        tvShow.TvShowRating = ratingValue;
                        tvShow.IsFinished = isFinished;
                        tvShow.Observations = observations;
                        db.SaveChanges();

                        //Save new cover
                        if (newCoverPath != null)
                        {
                            var coverResult = EditCover(tvShow.TvShowId, newCoverPath.Item2, newCoverPath.Item1, db);

                            if (coverResult == OutputTypeValues.Ok)
                                return OutputTypeValues.Ok;
                            else
                                return coverResult;
                        }

                        return OutputTypeValues.Ok;

                    }

                    return OutputTypeValues.TvShowNotFound;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error editing tv show -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        public int? GetTvShowsCount(string searchTitle, int? searchYear, bool searchFavorite, int? searchRating, bool searchFinished)
        {
            Console.WriteLine("TvShowService.GetTvShowsCount: ENTER");

            try
            {
                using (var db = new NorthwindContext())
                {
                    var query = (from t in db.TvShows
                                 where !t.Deleted
                                 select new
                                 {
                                     t.TvShowTitle,
                                     t.TvShowYear,
                                     t.IsFavorite,
                                     t.TvShowRating,
                                     t.IsFinished
                                 });

                    //Apply filters
                    if (!String.IsNullOrEmpty(searchTitle))
                    {
                        query = query.Where(t => t.TvShowTitle.ToLower().Contains(searchTitle.ToLower().Trim()));
                    }
                    if (searchYear.HasValue && searchYear.Value > 1979)
                    {
                        query = query.Where(t => t.TvShowYear == searchYear.Value);
                    }
                    if (searchFavorite)
                    {
                        query = query.Where(t => t.IsFavorite);
                    }
                    if (searchRating.HasValue)
                    {
                        query = query.Where(t => t.TvShowRating == searchRating.Value);
                    }
                    if (searchFinished)
                    {
                        query = query.Where(t => t.IsFinished);
                    }

                    Console.WriteLine("TvShowService.GetTvShowsCount: EXIT");
                    return query.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting tvshows count -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("TvShowService.GetTvShowsCount: EXIT");
            return null;
        }


        //Hidden methods
        private Tuple<string, string> SaveNewTvShowCover(string newName, string filePath)
        {
            Console.WriteLine("TvShowService.SaveNewTvShowCover: ENTER");

            try
            {   //Verify existence and get directory
                string coversFolder = Utils.GetControlWatchTvShowsFolder();

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

                    Console.WriteLine("TvShowService.SaveNewTvShowCover: EXIT");
                    return Tuple.Create(newCoverPath, newFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR Save new tvshow cover ->  " + ex.Message);
                return null;
            }

            Console.WriteLine("TvShowService.SaveNewTvShowCover: EXIT");
            return null;
        }

        private OutputTypeValues CreateCover(int tvShowId, string tvShowCover, string tvShowCoverPath, NorthwindContext db)
        {
            Console.WriteLine("TvShowService.CreateCover: ENTER");
            if (tvShowId < 0 || String.IsNullOrWhiteSpace(tvShowCover))
                return OutputTypeValues.DataError;

            try
            {
                if (db.TvShowCovers.Any(c => c.TvShowId == tvShowId))
                    return OutputTypeValues.AlreadyExistsCover;

                TvShowCover cover = new TvShowCover()
                {
                    TvShowId = tvShowId,
                    CoverName = tvShowCover,
                    CoverPath = tvShowCoverPath,
                    CreateDate = DateTime.UtcNow,
                    Deleted = false
                };

                db.TvShowCovers.Add(cover);
                db.SaveChanges();

                return OutputTypeValues.Ok;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating new tvshow cover -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }

        private OutputTypeValues EditCover(int tvShowId, string tvShowCover, string tvShowCoverPath, NorthwindContext db)
        {
            Console.WriteLine("TvShowService.EditCover: ENTER");
            if (tvShowId < 0 || String.IsNullOrWhiteSpace(tvShowCoverPath))
                return OutputTypeValues.DataError;

            try
            {
                var existingCover = db.TvShowCovers.Where(c => c.TvShowId == tvShowId && !c.Deleted).FirstOrDefault();
                if (existingCover != null)
                {
                    //Remove old cover
                    if (!String.IsNullOrEmpty(existingCover.CoverPath) && File.Exists(existingCover.CoverPath))
                        File.Delete(existingCover.CoverPath);

                    //Update movie cover
                    existingCover.CoverName = tvShowCover;
                    existingCover.CoverPath = tvShowCoverPath;
                    db.SaveChanges();

                    Console.WriteLine("TvShowService.EditCover: EXIT");
                    return OutputTypeValues.Ok;
                }

                return OutputTypeValues.MovieCoverNotFound;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error editing tvshow cover -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                return OutputTypeValues.Error;
            }
        }
    }
}
