using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XamarinLOBFormService.DataObjects;

namespace XamarinLOBFormService.Models
{
    public class UserLoginResultModel
    {
        public string AccessToken { get; set; } = "";
        public MyUser MyUser { get; set; }
    }
}