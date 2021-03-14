using AutoMapper;
using CrownBlog.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Filters
{
    /// <summary>
    /// 授權驗證篩選器
    /// </summary>
    public class AuthFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 設定檔
        /// </summary>
        private readonly IConfiguration Config;

        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpContextAccessor HttpContextAccessor;

        IMapper _mapper { get; }

        BlogContext BlogContext { get; }


        public AuthFilter(BlogContext blogContext, IConfiguration config, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            Config = config;
            HttpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            BlogContext = blogContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var message = $"一名使用者在{System.DateTime.Now}時，進入{context.Controller}的{context.RouteData.Values["action"]} Action";
            System.Diagnostics.Debug.WriteLine(message, "Action Filter Log");

            var loginUrl = "/blog/login";
            
            var cookie = HttpContextAccessor.HttpContext.Request.Cookies["CrownAuth"];


            if (string.IsNullOrEmpty(cookie))
            {
                context.Result = new RedirectResult(loginUrl);
            }
            else
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"User at {System.DateTime.Now} Auth Success!", "Action Filter");
                }
                catch
                {
                    context.Result = new RedirectResult(loginUrl);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
