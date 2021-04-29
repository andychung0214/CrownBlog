using CrownBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CrownBlog.Controllers
{
    [Route("api/error")]
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public ActionResult<ErrorResponseVM> Error()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    new ErrorResponseVM()
                    {
                        errorno = 999,
                        message = ex.Error.Message
                    });
            }
            else
            {
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    new ErrorResponseVM()
                    {
                        errorno = 999,
                        message = "ERROR OCCURRED!"
                    });
            }
        }
    }
}
