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
    public class DepartmentController : ApiController
    {
        private XamarinLOBFormContext Context = new XamarinLOBFormContext();
        APIResult fooResult = new APIResult();

        // GET: api/Project
        public APIResult Get()
        {
            var fooList = new List<DepartmentModel>();
            for (int i = 0; i < 10; i++)
            {
                var fooDepartmentModel = new DepartmentModel()
                {
                    DepartmentName = $"部門{i}",
                };
                fooList.Add(fooDepartmentModel);
            }
            fooResult.Success = true;
            fooResult.Message = "";
            fooResult.TokenFail = false;
            fooResult.Payload = fooList;
            return fooResult;
        }
    }
}
