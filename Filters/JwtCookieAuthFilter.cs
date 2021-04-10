using AdvantechConnect.Models.DTO;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CrownBlog.Filters
{
    /// <summary>
    /// 授權驗證篩選器
    /// </summary>
    public class JwtCookieAuthFilter: ActionFilterAttribute
    {
        /// <summary>
        /// 設定檔
        /// </summary>
        private readonly IConfiguration Config;

        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpContextAccessor HttpContextAccessor;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpContextAccessor"></param>
        public JwtCookieAuthFilter(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Config = configuration;
            HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 執行程序
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = HttpContextAccessor.HttpContext.Request;
            var cookie = request.Cookies["ConnectAuth"];

            //string accessToken = string.Empty;
            //AuthenticationHeaderValue authorization =context.HttpContext.Request.a

            //if (authorization != null && !string.IsNullOrEmpty(authorization.Parameter))
            //{
            //    accessToken = authorization.Parameter;
            //}

            //string token = GenerateAccessToken();

            //var uu = JsonConvert.SerializeObject(userInfo)


            //var cookie = context.HttpContext.Request.Cookies["CrownAuth"];
            //var cookie = context.HttpContext.Request.Headers.Values.Select( o => o.coo;

            //var request = HttpContextAccessor.HttpContext.Request;
            //System.Collections.ObjectModel.Collection<CookieHeaderValue> cookieCollection = request.Cookies;

            //HttpCookieCollecxtion MyCookieCollection = new HttpCookieCollection();
            //HttpCookie MyCookie = new HttpCookie("LastVisit");
            //MyCookie.Value = DateTime.Now.ToString();
            //MyCookieCollection.Add(MyCookie);


            //var cookie = context.HttpContext.Response.Cookies.Append("", "CrownAuth");
            if (string.IsNullOrEmpty(cookie))
            {
                var callback = WebUtility.UrlEncode($"{HttpContextAccessor.HttpContext.Request.Scheme}://{HttpContextAccessor.HttpContext.Request.Host}/Auth");
                context.Result = new RedirectResult($"{Config["GBLM:LoginUrl"]}/?callback={callback}");
            }
            else
            {
                try
                {
                    var userInfo = JWT.Decode<AccessTokenPayloadDTO>(cookie, Encoding.UTF8.GetBytes(Config["JwtKey"]), JwsAlgorithm.HS256);
                    var claims = new List<Claim>()
                    {
                        new Claim("UserInfo", JsonConvert.SerializeObject(userInfo)),
                    };

                    context.HttpContext.User = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims) });
                }
                catch
                {
                    var callback = WebUtility.UrlEncode($"{HttpContextAccessor.HttpContext.Request.Scheme}://{HttpContextAccessor.HttpContext.Request.Host}/Auth");
                    context.Result = new RedirectResult($"{Config["GBLM:LoginUrl"]}/?callback={callback}");
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
