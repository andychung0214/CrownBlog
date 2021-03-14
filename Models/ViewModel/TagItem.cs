using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.ViewModel
{
    public class TagItem
    {
        public Guid TagId { get; set; }
        public Guid? ArticleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public int? Sequence { get; set; }
        public int? Count { get; set; }
    }
}
