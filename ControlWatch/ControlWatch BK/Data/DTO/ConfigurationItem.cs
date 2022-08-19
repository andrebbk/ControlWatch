using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Data.DTO
{
    public class ConfigurationItem
    {
        public int ConfigurationId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Deleted { get; set; }


        //Use to load fast view data
        public string MoviesCoversPathConfig { get; set; }

        public string TvShowsCoversPathConfig { get; set; }
    }
}
