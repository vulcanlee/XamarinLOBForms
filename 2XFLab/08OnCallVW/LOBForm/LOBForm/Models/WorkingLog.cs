using System;
using System.Collections.Generic;
using System.Text;

namespace LOBForm.Models
{
    /// <summary>
    /// 工作日誌
    /// </summary>
    public class WorkingLog : ICloneable
    {
        public int WorkingLogId { get; set; }
        public MyUser Owner { get; set; } = new MyUser();
        public DateTime LogDate { get; set; }
        public double Hours { get; set; }
        public virtual Project Project { get; set; } = new Project();
        public string Title { get; set; }
        public string Summary { get; set; }

        public WorkingLog Clone()
        {
            var fooObj = this.MemberwiseClone() as WorkingLog;
            if (fooObj.Project != null)
            {
                fooObj.Project = fooObj.Project.Clone();
            }
            if (fooObj.Owner != null)
            {
                fooObj.Owner = fooObj.Owner.Clone();
            }
            return fooObj;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
