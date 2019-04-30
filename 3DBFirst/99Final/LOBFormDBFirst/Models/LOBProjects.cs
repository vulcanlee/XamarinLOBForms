using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst
{
    public partial class LOBProjects
    {
        public Project ToProject()
        {
            return new Project()
            {
                ProjectId = this.ProjectId,
                ProjectName = this.ProjectName,
            };
        }
    }
}