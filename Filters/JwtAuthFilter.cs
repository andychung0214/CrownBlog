using AutoMapper;
using CrownBlog.DAL;
using Jose;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrownBlog.Filters
{
    public class JwtAuthFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 設定檔
        /// </summary>
        private readonly IConfiguration Config;

        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;
        AuthorizationHandlerContext _authHandlerContext;
        IMapper _mapper { get; }

        BlogContext BlogContext { get; }


        public JwtAuthFilter(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Config = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            string secret = "myJwtAuthDemo";//加解密的key,如果不一樣會無法成功解密
            var request = _httpContextAccessor.HttpContext.Request;

            _httpContextAccessor.HttpContext.Response.Cookies.Append("CrownAuth", "1234", new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddYears(100) });
            actionContext.HttpContext.Response.Cookies.Append("CrownAuth", "5678", new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddYears(100) });

            var cookie = _httpContextAccessor.HttpContext.Request.Cookies["CrownAuth"];
            var token = actionContext.HttpContext.Request.Cookies["CrownAuth"];
            if (!WithoutVerifyToken(request.HttpContext.Request.Path.ToString()))
            {
                if (cookie != null)
                {
                    throw new System.Exception("Lost Token");
                }
                else
                {
                    //解密後會回傳Json格式的物件(即加密前的資料)
                    var jwtObject = JWT.Decode<Dictionary<string, Object>>(
                    secret,
                    Encoding.UTF8.GetBytes(secret),
                    JwsAlgorithm.HS512);

                    if (IsTokenExpired(jwtObject["Exp"].ToString()))
                    {
                        throw new System.Exception("Token Expired");
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        //Login不需要驗證因為還沒有token
        public bool WithoutVerifyToken(string requestUri)
        {
            if (requestUri.EndsWith("/Login"))
                return true;
            return false;
        }

        //驗證token時效
        public bool IsTokenExpired(string dateTime)
        {
            return Convert.ToDateTime(dateTime) < DateTime.Now;
        }
    }
}
