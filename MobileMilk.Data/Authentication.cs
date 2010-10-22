using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileMilk.Data
{
    public class Authentication
    {
        public string Token { get; set; }
        public AuthenticationPermissions Permissions { get; set; }
        public RtmUser User { get; set; }
    }
}
