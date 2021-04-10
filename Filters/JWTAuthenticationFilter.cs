using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace CrownBlog.Filters
{
    public class JWTAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple { get; }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;
            string ACCESS_TOKEN_SECRET_KEY = GetSecretKey(request.RequestUri);

            if (authorization == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            if (authorization.Parameter == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            if (authorization.Scheme != "Bearer")
            {
                context.ErrorResult = new AuthenticationFailureResult("The scheme of authorization must be Bearer.", request);
                return;
            }

            var secretKeyBytes = Encoding.UTF8.GetBytes(ACCESS_TOKEN_SECRET_KEY);

            try
            {
                Dictionary<string, object> payload = Jose.JWT.Decode<Dictionary<string, object>>(authorization.Parameter, secretKeyBytes, Jose.JwsAlgorithm.HS256);

                if (!payload.ContainsKey("exp") || payload["exp"] == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Invalid token.", request);
                    return;
                }
                else
                {
                    long ticks;

                    if (!long.TryParse(payload["exp"].ToString(), out ticks))
                    {
                        context.ErrorResult = new AuthenticationFailureResult("Invalid token.", request);
                        return;
                    }
                    else
                    {
                        DateTime exp = new DateTime(ticks);

                        if (DateTime.UtcNow > exp)
                        {
                            context.ErrorResult = new AuthenticationFailureResult("Invalid token.", request);
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid token.", request);
                return;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            // Do Nothing
            return Task.FromResult(0);
        }

        private string GetSecretKey(Uri requestUri)
        {
            NameValueCollection qsCollection = HttpUtility.ParseQueryString(requestUri.Query);
            string authorizedClients = "2021ConnectWW";
            List<string> authorizedClientList = authorizedClients.Split(new char[] { ',' }).ToList();

            if (qsCollection.Count > 0 && qsCollection.AllKeys.Contains("client") && authorizedClientList.Contains(qsCollection["client"]))
            {
                return "QSPdN2zBXmYFHQ65ve7PawZNa35u5KGa";
            }
            else
            {
                if (requestUri.AbsolutePath.StartsWith("/api/iot-summit/"))
                {
                    return "IoT_Summit_2018_XAEWITMR";
                }
                else
                {
                    return "fkyrbswpc201908";
                }
            }
        }
    }
}
