using CrownBlog.Models.Web;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace CrownBlog.Controllers
{
    public class BaseController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor { get; }

        /// <summary>
        /// Full Url
        /// </summary>
        public string Page_Base_UnEncoder
        {
            get
            {
                return $"{Request.Scheme}://{Page_Domain}";
            }
        }
        /// <summary>
        /// Full Url
        /// </summary>
        public string Page_URL_UnEncoder
        {
            get
            {
                return $"{Request.Scheme}://{Page_Domain}{Request.Path}";
            }
        }

        /// <summary>
        /// 當前頁面URL
        /// </summary>
        public string Page_URL
        {
            get
            {
                return UrlEncoder.Default.Encode($"{Request.Scheme}://{Page_Domain}{Request.Path}");
            }
        }

        /// <summary>
        /// Domain
        /// </summary>
        public string Page_Domain
        {
            get
            {
                return ViewBag.Domain;
            }
        }


        /// <summary>
        /// 地區
        /// </summary>
        public string Region
        {
            get
            {
                return Page_Domain == "crownchung.tw" ? "TW" : "US";
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // <summary>
        /// Get UI Culture Language
        /// </summary>
        //protected string propPage_UICulture
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(Request.QueryString["Lang"]))
        //        {
        //            Session["Lang"] = Request.QueryString["lang"];
        //        }
        //        else
        //        {
        //            //Session["Lang"] = this.SiteInfo.Lang;
        //            Session["Lang"] = "en-us";
        //        }
        //        return Session["Lang"].ToString().Trim();
        //    }
        //}

        protected string GenerateAccessToken(int expMinutes = 60)
        {
            string token = string.Empty;
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "email", "andy6756@gmail.com" },
                { "pid", "andychung0214" },
                { "reg_process", true },
                { "exp", DateTime.UtcNow.AddMinutes(expMinutes).Ticks }
            };

            var secretKeyBytes = Encoding.UTF8.GetBytes("IoT_Summit_2018_XAEWITMR");
            token = Jose.JWT.Encode(payload, secretKeyBytes, Jose.JwsAlgorithm.HS256);

            return token;
        }

        //protected Dictionary<string, object> GetAccessTokenPayload(string token)
        //{
        //    var secretKeyBytes = Encoding.UTF8.GetBytes("IoT_Summit_2018_XAEWITMR");
        //    Dictionary<string, object> payload = null;

        //    try
        //    {
        //        payload = Jose.JWT.Decode<Dictionary<string, object>>(token, secretKeyBytes, Jose.JwsAlgorithm.HS256);
        //    }
        //    catch (Exception)
        //    {

        //    }

        //    return payload;
        //}

        protected string GetWebAccessToken(ClaimsPrincipal claimsPrincipal)
        {
            //System.Web.Http.Controllers.HttpRequestContext request;

            //ClaimsPrincipal principal = _httpContextAccessor.HttpContext.User as ClaimsPrincipal;
            //ClaimsPrincipal principal = (ClaimsPrincipal)request.Principal;
            //ClaimsPrincipal principal = HttpContext.User.Identity as ClaimsPrincipal;
            string token = string.Empty;
            if (null != claimsPrincipal)
            {
                foreach (Claim claim in claimsPrincipal.Claims)
                {
                    Console.Write("CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value + "</br>");
                    if (claim.Type == "AccessToken")
                    {
                        token = claim.Value;
                        break;
                    }
                }
            }
            //foreach (Claim claim in principal.Claims)
            //{
            //    if (claim.Type == "AccessToken")
            //    {
            //        token = claim.Value;
            //        break;
            //    }
            //}

            return token;
        }

        protected AccessTokenPayload GetAccessTokenPayload(string token)
        {
            //string client = GetQueryString("client", "Default");
            //string secretKey = "QSPdN2zBXmYFHQ65ve7PawZNa35u5KGa";
            string secretKey = "IoT_Summit_2018_XAEWITMR";
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            AccessTokenPayload payload = null;

            try
            {
                payload = Jose.JWT.Decode<AccessTokenPayload>(token, secretKeyBytes, Jose.JwsAlgorithm.HS256);
            }
            catch (Exception ex)
            {

            }

            return payload;
        }


        protected string MakeAccessToken(string type, int expMinutes = 1440, Dictionary<string, string> properties = null)
        {
            //string client = GetQueryString("client", "Default");
            string secretKey = "QSPdN2zBXmYFHQ65ve7PawZNa35u5KGa";
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            AccessTokenPayload payload = new AccessTokenPayload
            {
                Type = "makeAccessToken",
                FirstName = "Crown",
                LastName = "Chung",
                Properties = properties
            };

            if (expMinutes < 0)
            {
                payload.Exp = (new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            }
            else
            {
                payload.Exp = DateTime.UtcNow.AddMinutes(expMinutes).Ticks;
            }

            return Jose.JWT.Encode(payload, secretKeyBytes, Jose.JwsAlgorithm.HS256);
        }

        protected string GetQueryString(string key, bool isDecode = true)
        {
            string value = string.Empty;

            try
            {
                value = Request.Query[key].ToString().Trim();

                if (!string.IsNullOrEmpty(value) && isDecode)
                {
                    value = HttpUtility.UrlDecode(value);
                }
            }
            catch (Exception)
            {

            }

            return value;
        }
    }
}
