using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.ViewModel
{
    public class LoginModel
    {

        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}
