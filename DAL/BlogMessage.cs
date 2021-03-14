using System;
using System.Collections.Generic;

#nullable disable

namespace CrownBlog.DAL
{
    public partial class BlogMessage
    {
        public Guid MessageId { get; set; }
        public Guid? ArticleId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? Status { get; set; }
        public string Address { get; set; }
        public string IconUrl { get; set; }
    }
}
