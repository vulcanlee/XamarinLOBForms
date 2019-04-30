using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst.Models
{
    public class UserLoginResultModel
    {
        public string AccessToken { get; set; } = "";
        public MyUser MyUser { get; set; }
    }
}