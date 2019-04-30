using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LOBFormDBFirst.Controllers
{
    public class ProjectController : ApiController
    {
        private LOBFormEntities db = new LOBFormEntities();

        // GET: api/Projects
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "",
                TokenFail = false,
                Payload = db.LOBProjects.Select(x => new Project()
                {
                    ProjectId = x.ProjectId,
                    ProjectName = x.ProjectName,
                })
            };
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
