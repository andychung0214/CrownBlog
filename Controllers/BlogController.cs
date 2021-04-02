using AutoMapper;
using CrownBlog.BLL;
using CrownBlog.DAL;
using CrownBlog.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrownBlog.Filters;

using CrownBlog.Models;

namespace CrownBlog.Controllers
{
    public class BlogController : BaseController
    {
        BlogService BlogService { get; }

        private readonly BlogContext _context;

        /// <summary>
        /// AutoMapper
        /// </summary>
        IMapper Mapper { get; }

        IConfiguration _configuration;

        private readonly IServiceProvider _serviceProvider;

        [Obsolete]
        public BlogController(BlogContext blogContext,  IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, IServiceProvider serviceProvider) : base(httpContextAccessor)
        {
            BlogService = new BlogService(blogContext, configuration, null, mapper);
            _context = blogContext;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string searchString, string currentFilter, string keyword, int id, string sortOrder, int? pageNumber, int pageSize = 0)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            ViewBag.KeyWord = keyword;
            ViewBag.CurrentPageIndex = pageNumber <= 1 ? 1 : pageNumber;
            ViewBag.TotalPageCount = 1;

            ArticleModel vm = new ArticleModel();
            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //vm.ArticleModels = BlogService.GetArticles(pageNumber, pageSize, searchString);
                //vm.Messages = BlogService.GetMessages();
                //ArticleModel aa = BlogService.GetTags();

                IQueryable<BlogArticle> filterArticles = BlogService.GetFilterArticles(searchString);
                var tmpTotalCount = filterArticles.ToList().Count;

                IQueryable<BlogArticle> allArticles = BlogService.GetAllArticles();

                IQueryable<BlogArticle> topArticles = BlogService.GetTopArticles();

                List<TagItem> tags = BlogService.GetAllTags();
                //var tmpTotalCount = articleModel.ArticleModels.Count;

                if (pageNumber == 0)
                {
                    pageNumber = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 4;
                }

                PaginatedList<BlogArticle> paginatedList = await PaginatedList<BlogArticle>.CreateAsync(filterArticles, pageNumber ?? 1, pageSize);
                List<ArticleModel> dbArticles = new List<ArticleModel>();

                for (int Idx = 0; Idx < paginatedList.Count; Idx++)
                {
                    dbArticles.Add(new ArticleModel()
                    {
                        Id = paginatedList[Idx].Id,
                        Title = paginatedList[Idx].Title,
                        Description = paginatedList[Idx].Description,
                        BannerUrl = paginatedList[Idx].BannerUrl,
                        Abstract = paginatedList[Idx].Abstract == null ? "" : paginatedList[Idx].Abstract,
                        CreateDate = paginatedList[Idx].CreateDate.GetValueOrDefault(),
                        ModifyDate = paginatedList[Idx].ModifyDate.GetValueOrDefault(),
                        TagName = BlogService.Get_Tags_By(paginatedList[Idx].Id).Select(o => o.Name).FirstOrDefault(),
                        preArticleId = Idx != 0 ? paginatedList[Idx - 1].Id : new Guid(),
                        preArticleTitle = Idx != 0 ? paginatedList[Idx - 1].Title : string.Empty,
                        nextArticleId = Idx < paginatedList.Count - 1 ? paginatedList[Idx + 1].Id : new Guid(),
                        nextArticleTitle = Idx < paginatedList.Count - 1 ? paginatedList[Idx + 1].Title : string.Empty,
                        preArticleBannerURL = Idx != 0 ? paginatedList[Idx - 1].BannerUrl : string.Empty,
                        nextArticleBannerURL = Idx < paginatedList.Count - 1 ? paginatedList[Idx + 1].BannerUrl : string.Empty,

                    });

                    vm.ArticleModels = dbArticles;
                    vm.Calendars = ConvertDatesToModel(allArticles.ToList());
                    vm.Years = allArticles.Select(o => o.CreateDate.Value.Year).Distinct().ToList();
                    vm.Tags = tags;
                    vm.TopArticles = ConvertArticleEntitiesToModels(topArticles.ToList());

                    List<BlogTag> blogTags = BlogService.GetAllTagsBy(paginatedList[Idx].Id);
                    List<TagItem> displayTags = new List<TagItem>();

                    foreach (var exitTagItem in blogTags)
                    {
                        displayTags.Add(new TagItem
                        {
                            Name = exitTagItem.Name,
                            TagId = exitTagItem.TagId,
                            ArticleId = exitTagItem.ArticleId,
                            Description = exitTagItem.Description,
                            Status = exitTagItem.Status
                        });
                    }

                    vm.TagSelectedItem = displayTags;
                }

                ViewBag.TotalPageCount = (tmpTotalCount / pageSize) + (tmpTotalCount % pageSize == 0 ? 0 : 1);

                return View(vm);
            }
        }

        public IActionResult Details(Guid id = new Guid(), Guid pId = new Guid(), Guid nId = new Guid())
        {
            ArticleModel vm = BlogService.GetArticleById(id, pId, nId);
            if (vm == null || id == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.articleId = Guid.Parse(id.ToString());
                ViewBag.preArticleId = Guid.Parse(pId.ToString());
                ViewBag.nextArticleId = Guid.Parse(nId.ToString());

                //Article Tags
                List<BlogTag> blogTags = BlogService.GetAllTagsBy(id);
                List<string> tagNames = blogTags.Select(o => o.Name).ToList();
                vm.SelectedTags = tagNames;

                return View(vm);
            }
        }

        [TypeFilter(typeof(AuthFilter))]
        public IActionResult Create()
        {
            ArticleModel vm = new ArticleModel();

            List<TagItem> tags = BlogService.GetAllTags();
            vm.Tags = tags;


            return View(vm);
        }

        [TypeFilter(typeof(AuthFilter))]
        public async Task<IActionResult> Edit(Guid Id)
        {
            ViewBag.articleId = Id;
            if (Id == Guid.Empty)
            {
                return NotFound();
            }

            ArticleModel vm = await BlogService.GetArticleByIdAsync(Id);

            if (vm == null)
            {
                return NotFound();
            }

            //Right Nav Tags
            List<TagItem> tags = BlogService.GetAllTags();
            vm.Tags = tags;

            //Article Tags
            List<BlogTag> blogTags = BlogService.GetAllTagsBy(Id);
            List<string> tagNames = blogTags.Select(o => o.Name).ToList();
            vm.SelectedTags = tagNames;


            return View(vm);
        }

        public IActionResult Category(string categoryName, int year = 0, int month = 0)
        {
            ArticleModel vm = new ArticleModel();

            List<BlogArticle> allArticles = BlogService.GetAllArticles().ToList();
            List<BlogArticle> articles = new List<BlogArticle>();

            List<ArticleModel> cateogyrArticles = new List<ArticleModel>();
            if (!string.IsNullOrEmpty(categoryName))
            {
                List<BlogTag> tagCategory = BlogService.Get_Article_Tags_By(categoryName);
                List<string> articleIds = tagCategory.Select(o => o.ArticleId.ToString()).ToList();

                foreach (BlogArticle item in allArticles)
                {
                    if (articleIds.Contains(item.Id.ToString()))
                    {
                        cateogyrArticles.Add(ConvertArticleEntitiyToModel(item));
                    }
                }
                vm.ArticleModels = cateogyrArticles;

            }
            else if (year != 0 && month != 0)
            {
                articles = BlogService.GetAllArticlesByYearMonth(year, month);
                vm.ArticleModels = ConvertArticleEntitiesToModels(articles);
            }
            else if (year != 0)
            {
                articles = BlogService.GetAllArticlesByYear(year);
                vm.ArticleModels = ConvertArticleEntitiesToModels(articles);
            }
            else
            {
                vm.ArticleModels = ConvertArticleEntitiesToModels(allArticles);
            }


            List<TagItem> tags = BlogService.GetAllTags();
            vm.Tags = tags;

            vm.Calendars = ConvertDatesToModel(allArticles.ToList());
            vm.Years = allArticles.Select(o => o.CreateDate.Value.Year).Distinct().ToList();

            return View(vm);
        }

        public IActionResult Album()
        {
            PhotoAlbumModel vm = new PhotoAlbumModel();



            return View(vm);
        }

        [Route("/blog/album/details/{id}")]
        public async Task<IActionResult> AlbumDetails(Guid id)
        {
            PhotoAlbumModel vm = new PhotoAlbumModel();


            if (id == Guid.Empty)
            {
                return RedirectPermanent("/blog/album/");
            }
            else
            {
                ViewBag.AlbumId = id.ToString();
                vm.Gallerys = new List<GalleryItem>();
                vm.Gallerys = GetGallerysById(id);
                if (id == Guid.Parse("3b3c1224-0311-44f0-8ec5-a64fe18ddec2"))
                {
                    vm.Title = "四極點大環島";
                }
            }

            ////{Config["Synology:User"]}
            //var session = new SynologySession(new Uri($"{ _configuration["Synology:User"] }"));
            //session.Login();
            //var api = new SynologyApi(session);

            //session.LogOut();

            return View(vm);
        }

        private List<GalleryItem> GetGallerysById(Guid id)
        {
            List<GalleryItem> galleries = new List<GalleryItem>();
            if (id == Guid.Parse("3b3c1224-0311-44f0-8ec5-a64fe18ddec2"))
            {
                galleries.Add(new GalleryItem
                {
                    Title = "富貴角燈塔",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/north/DSC_0604.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/north/DSC_0604.JPG"
                });

                galleries.Add(new GalleryItem
                {
                    Title = "富貴角燈塔2",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/north/DSC_0608.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/north/DSC_0608.JPG"
                });
                galleries.Add(new GalleryItem
                {
                    Title = "三貂嶺燈塔",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/east/DSC_0627.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/east/DSC_0627.JPG"
                });

                galleries.Add(new GalleryItem
                {
                    Title = "三貂嶺燈塔2",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/east/IMG_8192.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/east/IMG_8192.JPG"
                });

                galleries.Add(new GalleryItem
                {
                    Title = "國聖燈塔",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/west/DSC_0822.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/west/DSC_0822.JPG"
                });

                galleries.Add(new GalleryItem
                {
                    Title = "國聖燈塔2",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/west/DSC_0823.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/west/DSC_0823.JPG"
                });

                galleries.Add(new GalleryItem
                {
                    Title = "鵝鑾鼻燈塔",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/south/IMG_8408.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/south/IMG_8408.JPG"
                });

                galleries.Add(new GalleryItem
                {
                    Title = "鵝鑾鼻燈塔2",
                    ImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/south/DSC_0791.JPG",
                    ThumbImgUrl = "http://andychung0214.synology.me/images/cycles/taiwan/south/DSC_0791.JPG"
                });
            }

            return galleries;
        }

        public IActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            ArticleModel vm = new ArticleModel();
            string token = "1qaz2wsx";
            //Response.Cookies.Append("CrownBlogAuth", token, new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddYears(100) });

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticle(BlogArticle blogArticle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(blogArticle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changed.  Try again, and if the problem persists see your system administrator.");
                return View(blogArticle);
            }
            return View(blogArticle);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid? articleId, string Title, BlogArticle blogArticle)
        {
            if (articleId == null)
            {
                return NotFound();
            }
            var articleToUpdate = await _context.BlogArticles.FirstOrDefaultAsync(a => a.Id == articleId);
            if (await TryUpdateModelAsync<BlogArticle>(articleToUpdate, "", a => a.Title, a => a.Description, a => a.Abstract))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Unable to save changed.  Try again, and if the problem persists see your system administrator.");
                    throw;
                }
            }
            return View(articleToUpdate);
        }


        private ArticleModel ConvertArticleEntitiyToModel(BlogArticle blogArticle)
        {
            ArticleModel entity = new ArticleModel();
            entity.Id = blogArticle.Id;
            entity.Title = blogArticle.Title;
            entity.Description = blogArticle.Description;
            entity.CreateDate = blogArticle.CreateDate.GetValueOrDefault();
            entity.ModifyDate = blogArticle.ModifyDate.GetValueOrDefault();
            entity.Abstract = blogArticle.Abstract;
            entity.Url = blogArticle.Url;
            entity.BannerUrl = blogArticle.BannerUrl;
            entity.Status = blogArticle.Status;
            

            return entity;
        }

        private List<ArticleModel> ConvertArticleEntitiesToModels(List<BlogArticle> blogArticles)
        {
            List<ArticleModel> entities = new List<ArticleModel>();
            if (blogArticles != null)
            {
                foreach (BlogArticle item in blogArticles)
                {
                    entities.Add(new ArticleModel()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Description = item.Description,
                        CreateDate = item.CreateDate.GetValueOrDefault(),
                        ModifyDate = item.ModifyDate.GetValueOrDefault(),
                        Abstract = item.Abstract,
                        BannerUrl = item.BannerUrl,
                        Url = item.Url
                    });
                }
            }
            return entities;
        }

        private List<CalendarItem> ConvertDatesToModel(List<BlogArticle> blogArticles)
        {
            List<CalendarItem> dates = new List<CalendarItem>();

            var filterDates = blogArticles
                .GroupBy(o => new { o.CreateDate.Value.Year, o.CreateDate.Value.Month })
                .Select(o => new { o.Key.Year, o.Key.Month, Count = o.Count() }).ToList();


            foreach (var item in filterDates)
            {
                string monthRomma = ConvertMonthToMonthRomma(item.Month);

                string displayDate = $"{monthRomma}( {item.Count} )";

                dates.Add(
                    new CalendarItem
                    {
                        Year = item.Year,
                        Month = item.Month,
                        MonthRomma = monthRomma,
                        Count = item.Count,
                        DisplayDate = displayDate
                    }
                );
            }

            return dates;

        }

        private string ConvertMonthToMonthRomma(int month)
        {
            string MonthRomma = string.Empty;
            switch (month)
            {
                case 1:
                    MonthRomma = "January";
                    break;
                case 2:
                    MonthRomma = "February";
                    break;
                case 3:
                    MonthRomma = "March";
                    break;
                case 4:
                    MonthRomma = "April";
                    break;
                case 5:
                    MonthRomma = "May";
                    break;
                case 6:
                    MonthRomma = "June";
                    break;
                case 7:
                    MonthRomma = "July";
                    break;
                case 8:
                    MonthRomma = "August";
                    break;
                case 9:
                    MonthRomma = "Septemper";
                    break;
                case 10:
                    MonthRomma = "October";
                    break;
                case 11:
                    MonthRomma = "November";
                    break;
                case 12:
                    MonthRomma = "December";
                    break;
                default:
                    break;
            }
            return MonthRomma;
        }



    }
}
