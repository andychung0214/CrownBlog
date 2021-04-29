using CrownBlog.Models.Web;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CrownBlog.Controllers
{
    public class BaseApiController : ApiController
    {
        private IHttpContextAccessor _httpContextAccessor { get; }

        public BaseApiController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private Dictionary<string, string> _QueryStrings;

        protected Dictionary<string, string> QueryStrings
        {
            get
            {
                if (_QueryStrings == null)
                {
                    this.GetQueryStrings(Request);
                }

                return _QueryStrings;
            }
        }

        protected string GetQueryString(string key, string defaultVal = "", bool decode = false)
        {
            var value = string.Empty;

            if (!string.IsNullOrEmpty(key))
            {
                this.QueryStrings.TryGetValue(key, out value);
            }

            if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(defaultVal))
            {
                value = defaultVal;
            }

            if (decode && !string.IsNullOrEmpty(value))
            {
                value = HttpUtility.UrlDecode(value);
            }

            return value;
        }

        private void GetQueryStrings(HttpRequestMessage request)
        {
            this._QueryStrings = request.GetQueryNameValuePairs()
                                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }


        public string GenerateToken()
        {
            string secret = "myJwtAuthDemo";//加解密的key,如果不一樣會無法成功解密
            Dictionary<string, Object> claim = new Dictionary<string, Object>();//payload 需透過token傳遞的資料
            claim.Add("Account", "jim");
            claim.Add("Company", "appx");
            claim.Add("Department", "rd");
            claim.Add("Exp", DateTime.Now.AddSeconds(Convert.ToInt32("100")).ToString());//Token 時效設定100秒
            var payload = claim;
            var token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512);//產生token
            return token;
        }

        protected string GetWebAccessToken()
        {
            ClaimsPrincipal principal = (ClaimsPrincipal)RequestContext.Principal;
            string token = string.Empty;

            foreach (Claim claim in principal.Claims)
            {
                if (claim.Type == "AccessToken")
                {
                    token = claim.Value;
                    break;
                }
            }

            return token;
        }

        protected AccessTokenPayload GetAccessTokenPayload(string token)
        {
            string client = GetQueryString("client", "Default");
            string secretKey = "QSPdN2zBXmYFHQ65ve7PawZNa35u5KGa";
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            AccessTokenPayload payload = null;

            try
            {
                payload = Jose.JWT.Decode<AccessTokenPayload>(token, secretKeyBytes, Jose.JwsAlgorithm.HS256);
            }
            catch (Exception)
            {

            }

            return payload;
        }


        protected string MakeAccessToken(string type, int expMinutes = 1440, Dictionary<string, string> properties = null)
        {
            string client = GetQueryString("client", "Default");
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


    }
}
