using System;
using System.Collections.Generic;

#nullable disable

namespace CrownBlog.DAL
{
    public partial class BlogArticle
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Abstract { get; set; }
        public string Url { get; set; }
        public int? Status { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? Sequence { get; set; }
        public int? Visitors { get; set; }
        public string BannerUrl { get; set; }
        public string IconUrl { get; set; }
    }
}
