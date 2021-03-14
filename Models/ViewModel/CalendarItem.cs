using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.ViewModel
{
    public class CalendarItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthRomma { get; set; }
        public int Count { get; set; }

        public string DisplayDate { get; set; }
    }
}
