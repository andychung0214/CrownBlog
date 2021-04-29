using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.Web
{
    public class AccessTokenPayload
    {
        public string Type { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProfileId { get; set; }
        public string OrderSN { get; set; }
        public string OrderNo { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long Exp { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
