using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LOBFormDBFirst.Controllers
{
    public class DepartmentController : ApiController
    {
        APIResult fooResult = new APIResult();
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
