using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.DAL
{
    public class ListOptions
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool EnableCount { get; set; }
        public List<OrderBySet> OrderBySets { get; set; }

        public ListOptions()
        {
            PageIndex = 0;
            PageSize = 0;
            EnableCount = false;
            OrderBySets = new List<OrderBySet>();
        }
    }
}
