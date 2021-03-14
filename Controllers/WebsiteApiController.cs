using AutoMapper;
using CrownBlog.BLL;
using CrownBlog.DAL;
using CrownBlog.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Controllers
{
    [Route("api/web")]
    public class WebsiteApiController : BaseController
    {
        BlogService BlogService { get; }

        IMapper Mapper { get; }

        [Obsolete]
        public WebsiteApiController(BlogContext blogContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(httpContextAccessor)
        {
            BlogService = new BlogService(blogContext, configuration, null, mapper);
        }

        [HttpGet]
        [Route("getArticles")]
        public  async Task<List<ArticleResponseBody>> GetAllArtciles(ArticleRequestBody requestBody)
        {
            try
            {
                ArticleResponseBody response = new ArticleResponseBody();
                List<ArticleResponseBody> responses = new List<ArticleResponseBody>();
                var articles = BlogService.GetAllArticles();

                foreach (var item in articles)
                {
                    responses.Add(new ArticleResponseBody { Abstract = item.Abstract, Description = item.Description, Title = item.Title });
                }


                return responses;

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("article")]
        public async Task<ActionResult> AddArticle(ArticleRequestBody requestBody)
        {
            if (requestBody == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                BlogArticle article = await BlogService.CreateArticle(requestBody);

                if (requestBody.TagSelectedStrings != null)
                {
                    List<string> selectTags = requestBody.TagSelectedStrings.Split(",").ToList();

                    List<BlogTag> totalTags = BlogService.GetAllBlogTags();

                    foreach (var tagItemName in selectTags)
                    {
                        //Add Tag
                        TagRequestBody tagRequestBody = new TagRequestBody();
                        tagRequestBody.TagId = Guid.NewGuid();
                        tagRequestBody.Name = tagItemName;
                        tagRequestBody.ArticleId = requestBody.Id;

                        await BlogService.CreateTag(tagRequestBody);
                    }
                }

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("article/{articleId}")]
        public async Task<ActionResult> UpdateArticle(Guid articleId, ArticleRequestBody requestBody)
        {
            //if (requestBody.SelectedTags == null && requestBody.SelectedTags.Count <=0)
            //{
            //    requestBody.SelectedTags = GetCheckboxListItems();
            //}

            if (requestBody == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {

                await BlogService.UpdateArticle(articleId, requestBody);

                if (requestBody.TagSelectedStrings != null)
                {
                    List<string> selectTags = requestBody.TagSelectedStrings.Split(",").ToList();
                    List<TagItem> selectedTags = new List<TagItem>();

                    List<BlogTag> totalTags = BlogService.GetAllBlogTags();

                    foreach (var tagItemName in selectTags)
                    {
                        selectedTags.Add(new TagItem { Name = tagItemName });

                        var tagIsActive = totalTags.Where(o => o.Name == tagItemName && o.ArticleId == articleId).FirstOrDefault();
                        if (tagIsActive != null)
                        {
                            //Update Tag
                            TagRequestBody tagRequestBody = new TagRequestBody();
                            tagRequestBody.Name = tagItemName;
                            tagRequestBody.ArticleId = articleId;

                            List<TagItem> dbExistTags = BlogService.Get_Tags_By(articleId).ToList();
                            IEnumerable<Guid> tagId = dbExistTags
                                   .Where(o => o.Name == tagItemName && o.ArticleId == articleId)
                                   .Select(o => o.TagId);

                            await BlogService.UpdateTag(tagId.FirstOrDefault(), tagRequestBody);
                        }
                        else
                        {
                            //Add Tag
                            TagRequestBody tagRequestBody = new TagRequestBody();
                            var dbExistTags = BlogService.Get_Tags_By(articleId);
                            var isExistTags = dbExistTags.Select(o => o.Name).Contains(tagItemName);
                            if (!isExistTags)
                            {
                                tagRequestBody.TagId = Guid.NewGuid();
                                tagRequestBody.Name = tagItemName;
                                tagRequestBody.ArticleId = articleId;

                                await BlogService.CreateTag(tagRequestBody);
                            }
                        }
                    }


                    List<TagItem> dbTags = BlogService.Get_Tags_By(articleId).ToList();

                    foreach (var tagItem in dbTags)
                    {
                        var isHaveTag = selectTags.Contains(tagItem.Name);
                        if (!isHaveTag)
                        {
                            // Delete Tag
                            IEnumerable<Guid> tagId = dbTags
                                   .Where(o => o.Name == tagItem.Name && o.ArticleId == articleId)
                                   .Select(o => o.TagId);
                            await BlogService.DeleteTagBy(tagId.FirstOrDefault());
                        }
                    }

                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        [HttpDelete]
        [Route("article/{articleId}")]
        public async Task<ActionResult> DelteArticle(Guid artilceId)
        {
            if (artilceId == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                await BlogService.DeleteArticleBy(artilceId);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
