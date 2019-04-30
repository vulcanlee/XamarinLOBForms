using System;
using System.Collections.Generic;
using System.Text;

namespace LOBForm.Models
{
    /// <summary>
    /// 專案名稱
    /// </summary>
    public class Project : ICloneable
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public Project Clone()
        {
            return this.MemberwiseClone() as Project;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
