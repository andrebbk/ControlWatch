using ControlWatch.Commons.Enums;
using ControlWatch.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Commons.Services
{
    public interface IConfigurationService
    {
        OutputTypeValues CreateOrEditCoversPathConfiguration(string moviesCoversPath, string tvShowsCoversPath);

        ConfigurationItem GetCurrentPathConfig();

        string GetCurrentCoverPath(EntityTypeValues entityType);

        IEnumerable<ConfigurationItem> GetAllConfigurations();
    }
}
