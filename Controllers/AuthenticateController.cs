using CrownBlog.Models;
using CrownBlog.TokenAuthentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ITokenManager tokenManager;
        public AuthenticateController(ITokenManager tokenManager)
        {
            this.tokenManager = tokenManager;
        }

        public IActionResult Authenticate(string user, string pwd)
        {
            if (tokenManager.Authenticate(user, pwd))
            {
                return Ok(new { Token = tokenManager.NewToken() });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseVM() { errorno = 500, message = "Fail Auth" });
            }

        }
    }
}
