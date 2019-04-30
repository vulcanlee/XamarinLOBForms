using AuthToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthToken.Controllers
{
    public class NoAuthController : ApiController
    {
        [HttpGet]
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "不需要提供存取權杖 Access Token 就能使用的 API",
                Payload = new string[] { "無須存取權杖1", "無須存取權杖2" }
            };
        }
    }
}
