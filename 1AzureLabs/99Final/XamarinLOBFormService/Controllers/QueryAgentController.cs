using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XamarinLOBFormService.DataObjects;
using XamarinLOBFormService.Models;

namespace XamarinLOBFormService.Controllers
{
    [Microsoft.Azure.Mobile.Server.Config.MobileAppController]
    [Filters.JwtAuth]
    public class QueryAgentController : ApiController
    {
        private XamarinLOBFormContext Context = new XamarinLOBFormContext();
        APIResult fooResult = new APIResult();

        public APIResult Post([FromBody] LAFAgentQuery lafAgentQuery)
        {
            IQueryable<MyUser> fooList;
            //if (string.IsNullOrEmpty(lafAgentQuery.DepartmentName)&&string.IsNullOrEmpty(lafAgentQuery.Name))
            //{

            //}
            if (string.IsNullOrEmpty(lafAgentQuery.DepartmentName))
            {
                if (string.IsNullOrEmpty(lafAgentQuery.Name))
                {
                    fooList = Context.MyUsers;
                }
                else
                {
                    fooList = Context.MyUsers.Where(x => x.Name.Contains(lafAgentQuery.Name));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(lafAgentQuery.Name))
                {
                    fooList = Context.MyUsers.Where(x => x.DepartmentName == lafAgentQuery.DepartmentName);
                }
                else
                {
                    fooList = Context.MyUsers.Where(x => x.Name.Contains(lafAgentQuery.Name) && x.DepartmentName == lafAgentQuery.DepartmentName);
                }
            }
            var fooLAFAgentReslutList = new List<LAFAgentReslut>();
            var pp = fooList.ToList();
            foreach (var item in fooList)
            {
                var fooItem = new LAFAgentReslut()
                {
                    DepartmentName = item.DepartmentName,
                    MyUserId = item.MyUserId,
                    Name = item.Name,
                };
                fooLAFAgentReslutList.Add(fooItem);
            }
            fooResult.Success = true;
            fooResult.Message = "";
            fooResult.TokenFail = false;
            fooResult.Payload = fooLAFAgentReslutList;
            return fooResult;
        }
    }
}
