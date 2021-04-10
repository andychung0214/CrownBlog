using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace CrownBlog.Filters
{
    public class JwtCookieAuthenticationFilter
    {
        private const string COOKIE_KEY = "AdvAWAT";

        public bool AllowMultiple { get; }

        public JwtCookieAuthenticationFilter()
        {

        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            System.Collections.ObjectModel.Collection<CookieHeaderValue> cookieCollection = request.Headers.GetCookies(COOKIE_KEY);

            if (cookieCollection.Count == 0)
            {
                context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
                return;
            }

            CookieHeaderValue cookies = cookieCollection.First();

            if (cookies == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
                return;
            }

            string accessToken = string.Empty;

            foreach (CookieState cookieState in cookies.Cookies)
            {
                if (cookieState.Name == COOKIE_KEY)
                {
                    accessToken = cookieState.Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
                return;
            }

            try
            {
                //string secretKey = System.Configuration.ConfigurationManager.AppSettings["ApiJwtCookieSecret"].ToString();
                //var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
                //var payload = Jose.JWT.Decode<Models.JwtCookiePayload>(accessToken, secretKeyBytes, Jose.JwsAlgorithm.HS256);

                //DateTime exp = new DateTime(payload.Exp);

                //if (DateTime.UtcNow > exp)
                //{
                //    throw new InvalidOperationException("Invalid token.");
                //}

                //List<Claim> claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Email, payload.Email),
                //    new Claim("PermissionGroupIdList", string.Join(",", payload.PermissionGroupIdList))
                //};
                //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "JwtCookie");
                //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new[] { claimsIdentity });

                //context.Principal = claimsPrincipal;
            }
            catch (Exception)
            {
                context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
                return;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            // Do Nothing
            return Task.FromResult(0);
        }
    }
}
