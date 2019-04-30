using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst
{
    public partial class LOBWorkingLogs
    {
        public WorkingLog ToWorkingLog()
        {
            return new WorkingLog()
            {
                WorkingLogId = this.WorkingLogId,
                Hours = this.Hours,
                LogDate = this.LogDate,
                Owner = this.LOBMyUsers.ToMyUsers(),
                Project = this.LOBProjects.ToProject(),
                Summary = this.Summary,
                Title = this.Title,
            };
        }
    }
}