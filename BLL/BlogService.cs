using AutoMapper;
using CrownBlog.DAL;
using CrownBlog.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.BLL
{
    public class BlogService
    {
        /// <summary>
        /// BlogContext
        /// </summary>
        BlogContext BlogContext { get; set; }

        /// <summary>
        /// 環境元件
        /// </summary>
        IHostingEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// 設定檔
        /// </summary>
        IConfiguration Config { get; set; }

        /// <summary>
        /// AutoMapper
        /// </summary>
        IMapper Mapper { get; }


        private List<ArticleModel> AllArticles = new List<ArticleModel>();
        //private List<ArticleModel> allArticles
        //{
        //    get
        //    {
        //        if (_blog == null)
        //        {
        //            _blog = new Crown.Entities.BL.Blog();
        //        }
        //        return _blog;
        //    }
        //}
        public BlogService(BlogContext blogContext, IConfiguration config, IHostingEnvironment hostingEnvironment, IMapper mapper)
        {
            BlogContext = blogContext;
            Config = config;
            HostingEnvironment = hostingEnvironment;
            Mapper = mapper;
        }

        public List<BlogTag> Get_Article_Tags_By(string categoryName)
        {
            List<BlogTag> totalTags = new List<BlogTag>();
            var tags = from m in BlogContext.BlogTags
                       where m.Name == categoryName
                       select m;



            foreach (BlogTag item in tags)
            {
                totalTags.Add(item);
            }

            return totalTags;
        }

        public List<BlogTag> GetAllTagsBy(Guid articleId)
        {
            List<BlogTag> totalTags = new List<BlogTag>();
            var tags = from m in BlogContext.BlogTags
                       where m.ArticleId == articleId
                       select m;

            foreach (BlogTag item in tags)
            {
                totalTags.Add(item);
            }

            return totalTags;
        }
        public List<TagItem> GetAllTags()
        {
            var totalTag = (from m in BlogContext.BlogTags
                            select m).ToList();

            //var tags = totalTag.GroupBy(gtag => new { gtag.Name })
            //           .Select(gtag => new
            //           {
            //               ArticleId = gtag.Key,
            //               Name = gtag.Key.Name,
            //               Desc = gtag.Key.
            //               Count = gtag.Select(z => z.Name).Distinct().Count()
            //           });

            var tags = from g in
                           from c in totalTag
                           group c by c.Name
                       orderby g.Key
                       select new { Name = g.Key, Count = g.Count() };

        List < TagItem > listBlogTags = new List<TagItem>();
            foreach (var item in tags)
            {
                listBlogTags.Add(new TagItem { Name = item.Name, Count = item.Count });
            }

            return listBlogTags;
        }

        public List<BlogTag> GetAllBlogTags()
        {
            var allTags = (from t in BlogContext.BlogTags
                           select t);
            return allTags.ToList();
        }

        public IQueryable<BlogArticle> GetAllArticles()
        {
            IQueryable<BlogArticle> articles = from m in BlogContext.BlogArticles
                                                    orderby m.CreateDate descending
                                                    select m;

            return articles;
        }

        public IQueryable<BlogArticle> GetTopArticles()
        {
            IQueryable<BlogArticle> articles = from m in BlogContext.BlogArticles
                                               orderby m.CreateDate
                                               where m.CreateDate.Value.Year == 2020 && m.CreateDate.Value.Month == 10
                                               select m;

            return articles;
        }

        public List<BlogArticle> GetAllArticlesByYear(int year)
        {
            List<BlogArticle> articles = (from m in BlogContext.BlogArticles
                                          orderby m.CreateDate descending
                                          where m.CreateDate.Value.Year == year
                                          select m).ToList();

            return articles;
        }

        public List<BlogArticle> GetAllArticlesByYearMonth(int year, int month)
        {
            List<BlogArticle> articles = (from m in BlogContext.BlogArticles
                                               orderby m.CreateDate descending
                                               where m.CreateDate.Value.Year == year && m.CreateDate.Value.Month == month
                                               select m).ToList();

            return articles;
        }

        /// <summary>
        /// Articles List
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IQueryable<BlogArticle> GetFilterArticles(string keyword)
        {
            IQueryable<BlogArticle> articles = from m in BlogContext.BlogArticles
                                               orderby m.CreateDate descending
                                               where m.Status == 1
                                               select m;

            if (!string.IsNullOrEmpty(keyword))
            {
                articles = articles.Where(s => s.Title.Contains(keyword) || s.Description.Contains(keyword) || s.Description.Contains(keyword));
            }

            return articles;
        }

        public ArticleModel GetDBArticles()
        {
            ArticleModel articleModel = new ArticleModel();
            List<ArticleModel> articles = new List<ArticleModel>();

            IQueryable<BlogArticle> totalArticles = from m in BlogContext.BlogArticles
                                                    orderby m.CreateDate
                                                    select m;

            foreach (var item in totalArticles)
            {
                articles.Add(new ArticleModel() { Id = item.Id, 
                    Title = item.Title, 
                    Abstract = item.Abstract, 
                    BannerUrl = item.BannerUrl, 
                    CreateDate = item.CreateDate, 
                    Description = item.Description });
            }

            articleModel.ArticleModels = articles;

            return articleModel;
        }

        public List<ArticleModel> GetArticles(int? pageNumber, int pageSize, string searchString)
        {
            var entities = new List<BlogArticle>();

            //if (pageNumber > 0 && pageSize > 0)
            //{
            //    if (pageNumber == 1)
            //    {
            //        entities = BlogContext.BlogArticles
            //                    .OrderBy(x => x.CreateDate)
            //                    .Take(pageSize)
            //                    .ToList();
            //    }
            //    else
            //    {
            //        entities = BlogContext.BlogArticles
            //                    .OrderBy(x => x.CreateDate)
            //                    .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            //                    .ToList();
            //    }

            //    pageNumber = 1;
            //    pageSize = 0;
            //}
            //else
            //{
            //    entities = BlogContext.BlogArticles
            //                   .OrderBy(x => x.CreateDate)
            //                   .ToList();
            //}

            if (!string.IsNullOrEmpty(searchString))
            {
                entities = entities.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString)).ToList();
            }

            if (entities.Count <= 0)
            {
                return null;
            }
            else
            {

                List<ArticleModel> articles = new List<ArticleModel>();

                for (int idx = 0; idx < entities.Count; idx++)
                {
                    articles.Add(
                        new ArticleModel
                        {
                            Id = entities[idx].Id,
                            Abstract = entities[idx].Abstract,
                            CreateDate = entities[idx].CreateDate,
                            BannerUrl = entities[idx].BannerUrl,
                            Description = entities[idx].Description,
                            Title = entities[idx].Title,
                            Url = entities[idx].Url,
                            Status = entities[idx].Status,
                            Sequence = entities[idx].Sequence,
                            ModifyDate = entities[idx].ModifyDate,
                            preArticleTitle = idx <= 0 ? null: entities[idx - 1].Title,
                            nextArticleTitle = idx == entities.Count -1 ? null : entities[idx + 1].Title,
                            preArticleId = idx <= 0 ? new Guid() : entities[idx - 1].Id,
                            nextArticleId = idx == entities.Count - 1 ? new Guid() : entities[idx + 1].Id
                        }
                    );
                }

               
                AllArticles = articles;
                return articles;
            }
        }

        public ArticleModel GetArticleById(Guid articleId = new Guid(), Guid pId = new Guid(), Guid nId = new Guid())
        {
            ArticleModel article = new ArticleModel();

            //List<ArticleModel> articles = GetArticles();

            //if (pId == new Guid())
            //{
            //    List<ArticleModel> prevArticle = GetArticles();
            //    prevArticle = articles.Where(o => o.Id == articleId).ToList();
            //    pId = prevArticle.Select(o => o.preArticleId).FirstOrDefault();
            //}

            //if (nId == new Guid())
            //{
            //    List<ArticleModel> nextArticle = GetArticles();
            //    nextArticle = articles.Where(o => o.Id == articleId).ToList();
            //    nId = nextArticle.Select(o => o.nextArticleId).FirstOrDefault();
            //}

            var entity = BlogContext.BlogArticles
                                    .Where(o => o.Id == articleId)
                                    .FirstOrDefault();
            if (entity != null)
            {
                article = new ArticleModel()
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    CreateDate = entity.CreateDate,
                    BannerUrl = entity.BannerUrl,
                    Abstract = entity.Abstract,
                    Status = entity.Status,
                    preArticleId = pId,
                    preArticleTitle = BlogContext.BlogArticles
                                    .Where(o => o.Id == pId)
                                    .Select(o => o.Title)
                                    .FirstOrDefault(),
                    nextArticleId = nId,
                    nextArticleTitle = BlogContext.BlogArticles
                                    .Where(o => o.Id == nId)
                                    .Select(o => o.Title)
                                    .FirstOrDefault(),
                };

            }

            return article;
        }

        public async Task<ArticleModel> GetArticleByIdAsync(Guid articleId = new Guid(), Guid pId = new Guid(), Guid nId = new Guid())
        {
            ArticleModel article = new ArticleModel();


            var entity = BlogContext.BlogArticles
                                    .Where(o => o.Id == articleId)
                                    .FirstOrDefault();
            if (entity != null)
            {
                article = new ArticleModel()
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    CreateDate = entity.CreateDate,
                    BannerUrl = entity.BannerUrl,
                    Abstract = entity.Abstract,
                    Status = entity.Status,
                    preArticleId = pId,
                    preArticleTitle = BlogContext.BlogArticles
                                    .Where(o => o.Id == pId)
                                    .Select(o => o.Title)
                                    .FirstOrDefault(),
                    nextArticleId = nId,
                    nextArticleTitle = BlogContext.BlogArticles
                                    .Where(o => o.Id == nId)
                                    .Select(o => o.Title)
                                    .FirstOrDefault(),
                };

            }

            return article;
        }

        public List<TagItem> GetTags()
        {
            var entities = BlogContext.BlogTags
                            .OrderBy(x => x.Name)
                            .ToList();
            if (entities.Count <= 0)
            {
                return null;
            }
            else
            {
                List<TagItem> tags = new List<TagItem>();
                foreach (var entity in entities)
                {
                    //var tagVM = Mapper.Map<TagItem>(entity);
                    tags.Add(
                        new TagItem
                        {
                            TagId = entity.TagId,
                            Name = entity.Name,
                            Description = entity.Description
                        }
                    );

                }
                return tags;
            }
        }

        public List<TagItem> Get_Tags_By(Guid id)
        {
            List<TagItem> totalTags = new List<TagItem>();

            var entities = (from o in BlogContext.BlogTags
                       where o.ArticleId == id
                       select o).ToList();

            foreach (var entity in entities)
            {
                if (entity != null && !totalTags.Select(o => o.Name).ToList().Contains(entity.Name))
                {
                    totalTags.Add(new TagItem {
                        ArticleId = entity.ArticleId,
                        Name = entity.Name,
                        Count = entity.Count,
                        Description = entity.Description,
                        Sequence = entity.Sequence,
                        Status = entity.Status,
                        TagId = entity.TagId
                    });
                }
            }

            return totalTags;
        }

        public List<MessageItem> GetMessages()
        {
            var entities = BlogContext.BlogMessages
                            .OrderBy(x => x.Name)
                            .ToList();
            if (entities.Count <= 0)
            {
                return null;
            }
            else
            {
                List<MessageItem> messages = new List<MessageItem>();
                foreach (var entity in entities)
                {
                    //var tagVM = Mapper.Map<TagItem>(entity);
                    messages.Add(
                        new MessageItem
                        {
                            MessageId = entity.MessageId,
                            Name = entity.Name,
                            Comment = entity.Comment,
                            Subject = entity.Subject
                        }
                    );

                }
                return messages;
            }
        }


        #region BlogArticle

        public async Task<BlogArticle> CreateArticle(ArticleRequestBody articleInfo)
        {
            //var article = new BlogArticle
            //{
            //    Id = articleId,
            //    Title = title,
            //    Description = desc,
            //    Abstract = abstractText,
            //    CreateDate = createdDate.GetValueOrDefault() == new DateTime() ? DateTime.Now : createdDate
            //};
            var article = Mapper.Map<BlogArticle>(articleInfo);

            try
            {
                BlogContext.BlogArticles.Add(article);
                await BlogContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

            return article;
        }

        public async Task UpdateArticle(Guid articleId, ArticleRequestBody articleInfo)
        {
            var articleEntitiy = BlogContext.BlogArticles.Where(a => a.Id == articleId).FirstOrDefault();

            if (articleEntitiy == null)
            {
                throw new InvalidOperationException($"Connot find the article with article id: {articleId}");
            }
            else
            {
                try
                {

                    articleEntitiy.Title = articleInfo.Title;
                    articleEntitiy.Abstract = articleInfo.Abstract;
                    articleEntitiy.Description = !string.IsNullOrEmpty(articleInfo.Description) ? articleInfo.Description : articleEntitiy.Description;
                    articleEntitiy.CreateDate = articleInfo.CreateDate.GetValueOrDefault() == new DateTime() ? articleEntitiy.CreateDate : articleInfo.CreateDate.GetValueOrDefault();
                    articleEntitiy.ModifyDate = articleInfo.ModifyDate.GetValueOrDefault() == new DateTime() ? articleEntitiy.CreateDate : articleInfo.ModifyDate.GetValueOrDefault(); ;
                    articleEntitiy.BannerUrl = !string.IsNullOrEmpty(articleInfo.BannerUrl) ? articleInfo.BannerUrl : "";
                    articleEntitiy.Status = (articleInfo.Status != 1 && articleInfo.Status != 0) ? 0 : articleInfo.Status;

                    //var articleEntitiy = Mapper.Map<BlogArticle>(articleInfo);


                    BlogContext.BlogArticles.Update(articleEntitiy);
                    await BlogContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task DeleteArticleBy(Guid articleId)
        {
            var articleEntitiy = BlogContext.BlogArticles.Where(a => a.Id == articleId).FirstOrDefault();

            try
            {

                if (articleEntitiy == null)
                {
                    throw new InvalidOperationException($"Connot find the article with article id: {articleId}");
                }
                else
                {
                    BlogContext.BlogArticles.Remove(articleEntitiy);
                    await BlogContext.SaveChangesAsync();
                }

                var tagEntity = BlogContext.BlogTags.Where(a => a.ArticleId == articleId).FirstOrDefault();

                if (tagEntity == null)
                {
                    throw new InvalidOperationException($"Connot find the article with [tag] article id: {articleId}");
                }
                else
                {
                    BlogContext.BlogTags.Remove(tagEntity);
                    await BlogContext.SaveChangesAsync();
                }

                var messageEntity = BlogContext.BlogMessages.Where(m => m.ArticleId == articleId).FirstOrDefault();

                if (messageEntity == null)
                {
                    throw new InvalidOperationException($"Connot find the article with [message] article id: {articleId}");
                }
                else
                {
                    BlogContext.BlogMessages.Remove(messageEntity);
                    await BlogContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Tags

        public async Task<BlogTag> CreateTag(TagRequestBody tagInfo)
        {
            var tag = Mapper.Map<BlogTag>(tagInfo);

            try
            {
                BlogContext.BlogTags.Add(tag);
                await BlogContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            return tag;
        }

        public async Task UpdateTag(Guid tagId, TagRequestBody tagInfo)
        {
            var tagEntitiy = BlogContext.BlogTags.Where(a => a.TagId == tagId).FirstOrDefault();

            if (tagEntitiy == null)
            {
                throw new InvalidOperationException($"Connot find the tag with tagId: {tagId}");
            }
            else
            {
                tagEntitiy.Name = !string.IsNullOrEmpty(tagInfo.Name) ? tagInfo.Name : tagEntitiy.Name;
                tagEntitiy.Description = !string.IsNullOrEmpty(tagInfo.Description) ? tagInfo.Description : tagEntitiy.Description;
                tagEntitiy.Status = tagInfo.Status != 0 ? 1 : 0;
                tagEntitiy.Sequence = tagInfo.Sequence.GetValueOrDefault() != 0 ? tagInfo.Sequence : tagEntitiy.Sequence;
                tagEntitiy.TagId = tagInfo.TagId != Guid.Empty ? tagInfo.TagId : tagEntitiy.TagId;
                tagEntitiy.ArticleId = tagInfo.ArticleId != Guid.Empty ? tagInfo.ArticleId : tagEntitiy.ArticleId;

                //var articleEntitiy = Mapper.Map<BlogArticle>(articleInfo);

                try
                {
                    BlogContext.BlogTags.Update(tagEntitiy);
                    await BlogContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task DeleteTagBy(Guid tagId)
        {
            var tagEntity = BlogContext.BlogTags.Where(a => a.TagId == tagId).FirstOrDefault();

            if (tagEntity == null)
            {
                throw new InvalidOperationException($"Connot find the tag with tagIid: {tagId}");
            }
            else
            {
                try
                {
                    BlogContext.BlogTags.Remove(tagEntity);
                    await BlogContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        #endregion

    }
}
