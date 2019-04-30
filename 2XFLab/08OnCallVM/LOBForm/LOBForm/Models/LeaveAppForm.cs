using System;
using System.Collections.Generic;
using System.Text;

namespace LOBForm.Models
{
    /// <summary>
    /// 請假單
    /// </summary> 
    public class LeaveAppForm : ICloneable
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
        public string LeaveCause { get; set; }
        public string FormsStatus { get; set; }
        public string ApproveResult { get; set; }

        public LeaveAppForm Clone()
        {
            var fooObject = this.MemberwiseClone() as LeaveAppForm;
            fooObject.Owner = Owner.Clone();
            return fooObject;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
