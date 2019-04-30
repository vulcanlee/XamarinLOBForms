using System;
using System.Collections.Generic;
using System.Text;

namespace LOBForm.Models
{
    public class UserLoginResultModel
    {
        public string AccessToken { get; set; } = "";
        public MyUser MyUser { get; set; } = new MyUser();
    }
}
