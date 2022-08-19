using ControlWatch.Commons.Enums;
using ControlWatch.Commons.Helpers;
using ControlWatch.Commons.Services;
using ControlWatch.Data;
using ControlWatch.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public OutputTypeValues CreateOrEditCoversPathConfiguration(string moviesCoversPath, string tvShowsCoversPath)
        {
            Console.WriteLine("ConfigurationService.CreateOrEditCoversPathConfiguration: ENTER");

            if (String.IsNullOrWhiteSpace(moviesCoversPath) || String.IsNullOrWhiteSpace(tvShowsCoversPath))
                return OutputTypeValues.DataError;

            try
            { 
                using (var db = new NorthwindContext())
                {
                    var mcConfig = db.Configurations.FirstOrDefault(c => c.Key == UtilsConstants.MoviesCoversPath && !c.Deleted);
                    if(mcConfig != null)
                    {
                        mcConfig.Value = moviesCoversPath.Trim();
                    }
                    else
                    {
                        mcConfig = new Models.Configuration()
                        {
                            Key = UtilsConstants.MoviesCoversPath,
                            Value = moviesCoversPath.Trim(),
                            CreateDate = DateTime.Now,
                            Deleted = false
                        };
                        db.Configurations.Add(mcConfig);
                    }

                    db.SaveChanges();


                    var tcConfig = db.Configurations.FirstOrDefault(c => c.Key == UtilsConstants.TvShowsCoversPath && !c.Deleted);
                    if (tcConfig != null)
                    {
                        tcConfig.Value = tvShowsCoversPath.Trim();
                    }
                    else
                    {
                        tcConfig = new Models.Configuration()
                        {
                            Key = UtilsConstants.TvShowsCoversPath,
                            Value = tvShowsCoversPath.Trim(),
                            CreateDate = DateTime.Now,
                            Deleted = false
                        };
                        db.Configurations.Add(tcConfig);
                    }

                    db.SaveChanges();

                    Console.WriteLine("ConfigurationService.CreateOrEditCoversPathConfiguration: EXIT");
                    return OutputTypeValues.Ok;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error CreateOrEditCoversPathConfiguration -> " + ex.ToString());
                Console.WriteLine(ex.Message);

                Console.WriteLine("ConfigurationService.CreateOrEditCoversPathConfiguration: EXIT");
                return OutputTypeValues.Error;
            }
        }

        public ConfigurationItem GetCurrentPathConfig()
        {
            Console.WriteLine("ConfigurationService.GetCurrentPathConfig: ENTER");
            ConfigurationItem output = new ConfigurationItem();

            try
            {
                using (var db = new NorthwindContext())
                {
                    var configs = db.Configurations.Where(c => !c.Deleted);
                    if(configs != null && configs.Any())
                    {
                        foreach(var configItem in configs)
                        {
                            if (configItem.Key == UtilsConstants.MoviesCoversPath) output.MoviesCoversPathConfig = configItem.Value;

                            if (configItem.Key == UtilsConstants.TvShowsCoversPath) output.TvShowsCoversPathConfig = configItem.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetCurrentPathConfig -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("ConfigurationService.GetCurrentPathConfig: EXIT");
            return output;
        }

        public string GetCurrentCoverPath(EntityTypeValues entityType)
        {
            Console.WriteLine("ConfigurationService.GetCurrentCoverPath: ENTER");
            string output = null;

            try
            {
                string searchKey = entityType == EntityTypeValues.Movie ? UtilsConstants.MoviesCoversPath :
                        entityType == EntityTypeValues.TvShow ? UtilsConstants.TvShowsCoversPath : null;

                if (!String.IsNullOrEmpty(searchKey))
                {
                    using (var db = new NorthwindContext())
                    {
                        var query = db.Configurations.FirstOrDefault(c => !c.Deleted && c.Key == searchKey);
                        if(query != null)
                        {
                            output = query.Value;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetCurrentPathConfig -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("ConfigurationService.GetCurrentCoverPath: EXIT");
            return output;
        }

        public IEnumerable<ConfigurationItem> GetAllConfigurations()
        {
            Console.WriteLine("ConfigurationService.GetAllConfigurations: ENTER");
            List<ConfigurationItem> output = new List<ConfigurationItem>();

            try
            {
                using (var db = new NorthwindContext())
                {
                    var query = (from c in db.Configurations
                                 select new
                                 {
                                     c.ConfigurationId,
                                     c.Key,
                                     c.Value,
                                     c.CreateDate,
                                     c.Deleted
                                 })
                                 .OrderBy(c => c.ConfigurationId);

                    if (query != null && query.Any())
                    {
                        output = query.Select(c => new ConfigurationItem
                        {
                            ConfigurationId = c.ConfigurationId,
                            Key = c.Key,
                            Value = c.Value,
                            CreateDate = c.CreateDate,
                            Deleted = c.Deleted
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting all configurations -> " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("ConfigurationService.GetAllConfigurations: EXIT");
            return output;
        }        
    }
}
