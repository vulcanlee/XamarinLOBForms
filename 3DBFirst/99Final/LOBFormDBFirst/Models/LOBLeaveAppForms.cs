using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst
{
    public partial class LOBLeaveAppForms
    {
        public LeaveAppForm ToLeaveAppForm()
        {
            return new LeaveAppForm()
            {
                LeaveAppFormId = this.LeaveAppFormId,
                AgentId = this.AgentId,
                AgentName = this.AgentName,
                ApproveResult = this.ApproveResult,
                BeginDate = this.BeginDate,
                Category = this.Category,
                CompleteDate = this.CompleteDate,
                FormDate = this.FormDate,
                FormsStatus = this.FormsStatus,
                Hours = this.Hours,
                LeaveCause = this.LeaveCause,
                Owner = this.LOBMyUsers == null ? null : this.LOBMyUsers.ToMyUsers()
            };
        }
    }
}