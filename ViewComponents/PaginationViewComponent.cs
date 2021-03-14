using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CrownBlog.ViewComponents
{
    public class PaginationViewComponent : ViewComponent
    {
        protected readonly IHostingEnvironment Env;
        protected readonly CrownBlog.DAL.BlogContext blogContext;

        public PaginationViewComponent(IHostingEnvironment env, CrownBlog.DAL.BlogContext _blogContext)
        {
            Env = env;
            blogContext = _blogContext;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
