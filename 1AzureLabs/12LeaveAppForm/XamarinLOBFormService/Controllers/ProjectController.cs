using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XamarinLOBFormService.Models;

namespace XamarinLOBFormService.Controllers
{
    [Microsoft.Azure.Mobile.Server.Config.MobileAppController]
    public class ProjectController : ApiController
    {
        private XamarinLOBFormContext Context = new XamarinLOBFormContext();
        APIResult fooResult = new APIResult();

        // GET: api/Project
        public APIResult Get()
        {
            fooResult.Success = true;
            fooResult.Message = "";
            fooResult.TokenFail = false;
            fooResult.Payload = Context.Projects.ToList();
            return fooResult;
        }
    }
}
