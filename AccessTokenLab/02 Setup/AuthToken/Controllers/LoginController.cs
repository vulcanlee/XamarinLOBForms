using AuthToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthToken.Controllers
{
    public class LoginController : ApiController
    {
        [HttpGet]
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "",
                Payload = "Access Token"
            };
        }
    }
}
