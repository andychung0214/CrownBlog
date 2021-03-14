using System;
using System.Collections.Generic;

#nullable disable

namespace CrownBlog.DAL
{
    public partial class BlogActivity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
