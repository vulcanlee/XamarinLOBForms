using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst.Models
{
    /// <summary>
    /// 工作日誌
    /// </summary>
    public class WorkingLog
    {
        public int WorkingLogId { get; set; }
        public MyUser Owner { get; set; }
        public DateTime LogDate { get; set; }
        public double Hours { get; set; }
        public virtual Project Project { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
    }
}