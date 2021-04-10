using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace CrownBlog.Filters
{
    //public class AuthorizeJWT : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        var jwt = context.HttpContext.Request.Headers["JWT"];

    //        try
    //        {
    //            var json = new JwtBuilder()
    //                .WithSecret(File.ReadLines("").ToList().First())
    //                .MustVerifySignature()
    //                .Decode(jwt);

    //            var tokenDetails = JsonConvert.DeserializeObject<dynamic>(json);
    //        }
    //        catch (TokenExpiredException)
    //        {
    //            throw new Exception("Token is expired");
    //        }
    //        catch (SignatureVerificationException)
    //        {
    //            throw new Exception("Token signature invalid");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception("Token has been tempered with");
    //        }
    //    }
    //}
}
