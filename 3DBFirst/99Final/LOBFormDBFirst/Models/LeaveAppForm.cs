using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst.Models
{
    /// <summary>
    /// 請假單
    /// </summary>
    public class LeaveAppForm
    {
        public int LeaveAppFormId { get; set; }
        public virtual MyUser Owner { get; set; }
        //public int OwnerId { get; set; }
        public DateTime FormDate { get; set; }
        public string Category { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public double Hours { get; set; }
        public string AgentName { get; set; }
        public int AgentId { get; set; }
        public string LeaveCause { get; set; }
        public string FormsStatus { get; set; }
        public string ApproveResult { get; set; }
    }
}