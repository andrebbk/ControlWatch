using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Models
{
    public class Configuration
    {
        [Key]
        public int ConfigurationId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Deleted { get; set; }
    }
}
