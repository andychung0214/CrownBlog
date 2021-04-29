﻿using System;
using System.Collections.Generic;

#nullable disable

namespace CrownBlog.DAL
{
    public partial class Member
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AuthCode { get; set; }
        public bool IsAdmin { get; set; }
        public bool Status { get; set; }
    }
}
