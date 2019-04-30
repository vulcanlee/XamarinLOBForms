using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LOBFormDBFirst.Controllers
{
#if !DEBUG
    [Filters.JwtAuth]
#endif
    public class QueryAgentController : ApiController
    {
        private LOBFormEntities db = new LOBFormEntities();
        private APIResult fooResult = new APIResult();

        public async Task<APIResult> Post([FromBody] LAFAgentQuery lafAgentQuery)
        {
            IEnumerable<MyUser> fooList;
            if (string.IsNullOrEmpty(lafAgentQuery.DepartmentName))
            {
                if (string.IsNullOrEmpty(lafAgentQuery.Name))
                {
                    await db.LOBMyUsers.ToListAsync();
                    fooList = (await db.LOBMyUsers.ToListAsync()).Select(x => x.ToMyUsers());
                }
                else
                {
                    fooList = (await (db.LOBMyUsers.Where(x => x.Name.Contains(lafAgentQuery.Name)))
                        .ToListAsync()).Select(x => x.ToMyUsers());
                }
            }
            else
            {
                if (string.IsNullOrEmpty(lafAgentQuery.Name))
                {
                    fooList = (await (db.LOBMyUsers.Where(x => x.DepartmentName == lafAgentQuery.DepartmentName))
                        .ToListAsync()).Select(x => x.ToMyUsers());
                }
                else
                {
                    fooList = (await (db.LOBMyUsers.Where(x => x.Name.Contains(lafAgentQuery.Name) && x.DepartmentName == lafAgentQuery.DepartmentName))
                        .ToListAsync()).Select(x => x.ToMyUsers());
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
