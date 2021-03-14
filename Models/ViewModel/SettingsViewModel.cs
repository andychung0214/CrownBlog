using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.ViewModel
{
    public class SettingsViewModel
    {
        public string SynologyHost { get; set; }
        public int SynologyPort { get; set; }
        public string SynologyUser { get; set; }
        public string SynologyPass { get; set; }
        public bool UseSsl { get; set; }
    }
}
