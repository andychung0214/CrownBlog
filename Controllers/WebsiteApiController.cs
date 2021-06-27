using AutoMapper;
using CrownBlog.BLL;
using CrownBlog.DAL;
using CrownBlog.Filters;
using CrownBlog.Models;
using CrownBlog.Models.ViewModel;
using CrownBlog.Models.Web;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrownBlog.Controllers
{
    [Route("api/web")]
    public class WebsiteApiController : BaseController
    {
        BlogService BlogService { get; }

        IMapper _mapper { get; }

        [Obsolete]
        public WebsiteApiController(BlogContext blogContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(httpContextAccessor)
        {
            BlogService = new BlogService(blogContext, configuration, null, mapper);
            _mapper = mapper;
        }

        #region Articles API
        [HttpGet]
        [EnableCors("AllowAllGetPolicy")]
        [Route("articles")]
        public async Task<List<ArticleResponseBody>> GetAllArtciles(ArticleRequestBody requestBody)
        {
            //ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                ArticleResponseBody response = new ArticleResponseBody();
                List<ArticleResponseBody> responses = new List<ArticleResponseBody>();
                var articles = BlogService.GetAllArticles();

                foreach (var item in articles)
                {
                    responses.Add(
                        new ArticleResponseBody { 
                            Id = item.Id,
                            Abstract = item.Abstract, 
                            Description = item.Description, 
                            Title = item.Title,
                            CreateDate = item.CreateDate,
                            ModifyDate = item.ModifyDate,
                            BannerUrl = item.BannerUrl,
                            Focus = item.Focus,
                            Status = item.Status,
                            Url = item.Url
                    });
                }

                //HttpContext httpContext = new HttpContext();
                //.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                return responses;

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [EnableCors("AllowAllGetPolicy")]
        [Route("article/{id}")]
        public async Task<ArticleResponseBody> GetArtcileById(ArticleRequestBody requestBody, Guid id)
        {
            try
            {
                ArticleResponseBody response = new ArticleResponseBody();

                if (id == Guid.Empty)
                {
                    return new ArticleResponseBody();
                }

                var article = BlogService.GetArticleById(id);

                response.Id = article.Id;
                response.Abstract = article.Abstract;
                response.Description = article.Description;
                response.Title = article.Title;
                response.CreateDate = article.CreateDate;
                response.ModifyDate = article.ModifyDate;
                response.BannerUrl = article.BannerUrl;
                response.Focus = article.Focus;
                response.Status = article.Status;
                response.Url = article.Url;

                return response;

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
                return StatusCode(StatusCodes.Status404NotFound, requestBody);
            }

            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                if (requestBody.CreateDate == new DateTime() || requestBody.CreateDate == null)
                {
                    requestBody.CreateDate = System.DateTime.Now;
                }
                requestBody.Focus = false;
                HttpResponseMessage article = BlogService.CreateArticle(response, requestBody);

                //if (requestBody.TagSelectedStrings != null)
                //{
                //    List<string> selectTags = requestBody.TagSelectedStrings.Split(",").ToList();

                //    List<BlogTag> totalTags = BlogService.GetAllBlogTags();

                //    foreach (var tagItemName in selectTags)
                //    {
                //        //Add Tag
                //        TagRequestBody tagRequestBody = new TagRequestBody();
                //        tagRequestBody.TagId = Guid.NewGuid();
                //        tagRequestBody.Name = tagItemName;
                //        tagRequestBody.ArticleId = requestBody.Id;
                //        tagRequestBody.Status = 0;
                //        BlogService.CreateTag(tagRequestBody);
                //    }
                //}

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseVM() { errorno = 500, message = ex.ToString() });

            }
        }

        [HttpPut]
        [Route("article/{id}")]
        public async Task<ActionResult> UpdateArticle(Guid id, ArticleRequestBody requestBody)
        {

            if (requestBody == null || !ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status404NotFound, requestBody);
            }

            try
            {
                requestBody.ModifyDate = System.DateTime.Now;
                await BlogService.UpdateArticle(id, requestBody);

                if (requestBody.TagSelectedStrings != null)
                {
                    List<string> selectTags = requestBody.TagSelectedStrings.Split(",").ToList();
                    List<TagItem> selectedTags = new List<TagItem>();

                    List<BlogTag> totalTags = BlogService.GetAllBlogTags();

                    foreach (var tagItemName in selectTags)
                    {
                        selectedTags.Add(new TagItem { Name = tagItemName });

                        var tagIsActive = totalTags.Where(o => o.Name == tagItemName && o.ArticleId == id).FirstOrDefault();
                        if (tagIsActive != null)
                        {
                            //Update Tag
                            TagRequestBody tagRequestBody = new TagRequestBody();
                            tagRequestBody.Name = tagItemName;
                            tagRequestBody.ArticleId = id;

                            List<TagItem> dbExistTags = BlogService.Get_Tags_By(id).ToList();
                            IEnumerable<Guid> tagId = dbExistTags
                                   .Where(o => o.Name == tagItemName && o.ArticleId == id)
                                   .Select(o => o.TagId);

                            await BlogService.UpdateTag(tagId.FirstOrDefault(), tagRequestBody);
                        }
                        else
                        {
                            //Add Tag
                            TagRequestBody tagRequestBody = new TagRequestBody();
                            var dbExistTags = BlogService.Get_Tags_By(id);
                            var isExistTags = dbExistTags.Select(o => o.Name).Contains(tagItemName);
                            if (!isExistTags)
                            {
                                tagRequestBody.TagId = Guid.NewGuid();
                                tagRequestBody.Name = tagItemName;
                                tagRequestBody.ArticleId = id;

                                await BlogService.CreateTag(tagRequestBody);
                            }
                        }
                    }


                    List<TagItem> dbTags = BlogService.Get_Tags_By(id).ToList();

                    foreach (var tagItem in dbTags)
                    {
                        var isHaveTag = selectTags.Contains(tagItem.Name);
                        if (!isHaveTag)
                        {
                            // Delete Tag
                            IEnumerable<Guid> tagId = dbTags
                                   .Where(o => o.Name == tagItem.Name && o.ArticleId == id)
                                   .Select(o => o.TagId);
                            await BlogService.DeleteTagBy(tagId.FirstOrDefault());
                        }
                    }

                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseVM() { errorno = 500, message = ex.ToString() });
            }
        }

        [HttpDelete]
        [Route("article/{id}")]
        public async Task<ActionResult> DelteArticle(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                await BlogService.DeleteArticleBy(id);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        #endregion

        #region Tag API
        #endregion


        #region Message API

        [HttpPost]
        [Route("message")]
        [TokenAuthenticationFilter]
        public async Task<ActionResult> CreateMessage(MessageRequestBody requestBody)
        {
            if (requestBody == null || !ModelState.IsValid)
            {
                return NotFound(requestBody);
            }

            try
            {
                requestBody.MessageId = Guid.NewGuid();
                if (requestBody.ArticleId != Guid.Empty)
                {
                    requestBody.ArticleId = requestBody.ArticleId;
                }

                requestBody.CreateDate = System.DateTime.Now;
                BlogMessage user = await BlogService.CreateMessage(requestBody);

                return Ok($"Create Message ID Message done.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseVM() { errorno = 500, message = ex.ToString() });
            }
        }
        #endregion


        #region Member API
        [HttpGet]
        [EnableCors("AllowAllGetPolicy")]
        [Route("members")]
        public async Task<List<MemberResponseBody>> GetUsers()
        {
            try
            {
                List<MemberResponseBody> responses = new List<MemberResponseBody>();
                List<MemberModel> members = await BlogService.GetMembersAsync();

                foreach (var item in members)
                {
                    var result = _mapper.Map<MemberResponseBody>(item);
                    responses.Add(result);
                }

                return responses;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [EnableCors("AllowAllGetPolicy")]
        [Route("user/{id}")]
        public async Task<ActionResult> GetUserById(ArticleRequestBody requestBody, Guid id)
        {
            try
            {
                ArticleResponseBody response = new ArticleResponseBody();

                if (id == Guid.Empty)
                {
                    return StatusCode(StatusCodes.Status404NotFound, requestBody);
                }

                //var article = BlogService

                //response.Id = article.Id;
                //response.Abstract = article.Abstract;
                //response.Description = article.Description;
                //response.Title = article.Title;
                //response.CreateDate = article.CreateDate;
                //response.ModifyDate = article.ModifyDate;
                //response.BannerUrl = article.BannerUrl;
                //response.Focus = article.Focus;
                //response.Status = article.Status;
                //response.Url = article.Url;

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseVM() { errorno = 500, message = ex.ToString() });

            }
        }

        [HttpPost]
        [TokenAuthenticationFilter]
        [Route("member")]
        public async Task<ActionResult> CreateUser([FromBody] MemberRequestBody requestBody)
        {
            var claims = new List<Claim>();
            var claimsIdentity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //string token = base.GetWebAccessToken(claimsPrincipal);
            string token = base.GenerateAccessToken();
            AccessTokenPayload accessTokenPayload = GetAccessTokenPayload(token);

            if (accessTokenPayload == null)
            {
                //LogMessage(LogType.Info, "", "Unauthorized.");
                return StatusCode(StatusCodes.Status401Unauthorized, "Token is invalid.");
            }


            if (requestBody == null || !ModelState.IsValid)
            {
                return NotFound(requestBody);
            }

            try
            {
                requestBody.Id = Guid.NewGuid();
                //Member user = await BlogService.CreateMember(requestBody);

                return Ok($"Create ID: {requestBody.Id} Member done.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseVM() { errorno = 500, message = ex.ToString()  });
            }
        }

        [HttpPut]
        [Route("user/{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, ArticleRequestBody requestBody)
        {

            if (requestBody == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                requestBody.ModifyDate = System.DateTime.Now;
                await BlogService.UpdateArticle(id, requestBody);

                if (requestBody.TagSelectedStrings != null)
                {
                    List<string> selectTags = requestBody.TagSelectedStrings.Split(",").ToList();
                    List<TagItem> selectedTags = new List<TagItem>();

                    List<BlogTag> totalTags = BlogService.GetAllBlogTags();

                    foreach (var tagItemName in selectTags)
                    {
                        selectedTags.Add(new TagItem { Name = tagItemName });

                        var tagIsActive = totalTags.Where(o => o.Name == tagItemName && o.ArticleId == id).FirstOrDefault();
                        if (tagIsActive != null)
                        {
                            //Update Tag
                            TagRequestBody tagRequestBody = new TagRequestBody();
                            tagRequestBody.Name = tagItemName;
                            tagRequestBody.ArticleId = id;

                            List<TagItem> dbExistTags = BlogService.Get_Tags_By(id).ToList();
                            IEnumerable<Guid> tagId = dbExistTags
                                   .Where(o => o.Name == tagItemName && o.ArticleId == id)
                                   .Select(o => o.TagId);

                            await BlogService.UpdateTag(tagId.FirstOrDefault(), tagRequestBody);
                        }
                        else
                        {
                            //Add Tag
                            TagRequestBody tagRequestBody = new TagRequestBody();
                            var dbExistTags = BlogService.Get_Tags_By(id);
                            var isExistTags = dbExistTags.Select(o => o.Name).Contains(tagItemName);
                            if (!isExistTags)
                            {
                                tagRequestBody.TagId = Guid.NewGuid();
                                tagRequestBody.Name = tagItemName;
                                tagRequestBody.ArticleId = id;

                                await BlogService.CreateTag(tagRequestBody);
                            }
                        }
                    }


                    List<TagItem> dbTags = BlogService.Get_Tags_By(id).ToList();

                    foreach (var tagItem in dbTags)
                    {
                        var isHaveTag = selectTags.Contains(tagItem.Name);
                        if (!isHaveTag)
                        {
                            // Delete Tag
                            IEnumerable<Guid> tagId = dbTags
                                   .Where(o => o.Name == tagItem.Name && o.ArticleId == id)
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
        [Route("user/{id}")]
        public async Task<ActionResult> DelteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                await BlogService.DeleteArticleBy(id);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        #endregion
    }
}
