using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrownBlog.TokenAuthentication
{
    public interface ITokenManager
    {
        bool Authenticate(string username, string password);
        string NewToken();
        ClaimsPrincipal VerifyToken(string token);
    }
}
