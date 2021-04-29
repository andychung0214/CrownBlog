using System;
using System.Collections.Generic;

#nullable disable

namespace CrownBlog.DAL
{
    public partial class MultiLanguage
    {
        public string Project { get; set; }
        public string Keyword { get; set; }
        public string Lang { get; set; }
        public string Value { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
