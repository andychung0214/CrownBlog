using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.ViewModel
{
    public class ArticleModel
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

        public int CommentCount { get; set; }
        public string TagName { get; set; }
        public List<TagItem> Tags { get; set; }
        public List<MessageItem> Messages { get; set; }
        public List<int> Years { get; set; }
        public List<CalendarItem> Calendars { get; set; }

        public string preArticleTitle { get; set; }
        public string nextArticleTitle { get; set; }
        public Guid preArticleId { get; set; }
        public Guid nextArticleId { get; set; }

        public string preArticleBannerURL { get; set; }
        public string nextArticleBannerURL { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }

        public List<Guid> TagIds { get; set; }
        public List<ArticleModel> ArticleModels { get; set; }
        public List<ArticleModel> TopArticles { get; set; }

        public string TagSelectedStrings { get; set; }
        public List<string> SelectedTags { get; set; }
        public List<TagItem> TagSelectedItem { get; set; }

    }
}
