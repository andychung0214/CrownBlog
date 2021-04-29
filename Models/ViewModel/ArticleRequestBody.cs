using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.ViewModel
{
    public class ArticleRequestBody
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
        public bool? Focus { get; set; }


        public List<Guid> TagGuids { get; set; }
        public string TagSelectedStrings { get; set; }
        public List<string> SelectedTags { get; set; }
        public List<TagRequestBody> TagSelectedItem { get; set; }


    }
}
