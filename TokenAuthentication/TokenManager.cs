using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrownBlog.TokenAuthentication
{
    public class TokenManager : ITokenManager
    {
        private List<Token> listTokens;
        private JwtSecurityTokenHandler tokenHandler;
        private byte[] secretKey;

        public TokenManager()
        {
            listTokens = new List<Token>();
            tokenHandler = new JwtSecurityTokenHandler();
            secretKey = Encoding.UTF8.GetBytes("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
        }

        public bool Authenticate(string userName, string password)
        {
            if (!string.IsNullOrWhiteSpace(userName) &&
                !string.IsNullOrWhiteSpace(password) &&
                userName == "amTea" && 
                password == "02141012")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string NewToken()
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Crown Chung") }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);
            return jwtString;
        }

        public ClaimsPrincipal VerifyToken(string token)
        {
            var claims = tokenHandler.ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return claims;
        }
    }
}
