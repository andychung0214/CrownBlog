using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models
{
    public class ListResult<T> where T: class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }

        public ListResult()
        {
            PageIndex = 0;
            PageSize = 0;
            TotalCount = 0;
            Items = new List<T>();
        }
    }
}
