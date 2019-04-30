using System;
using System.Collections.Generic;
using System.Text;

namespace LOBForm.Models
{
    /// <summary>
    /// 請假類別
    /// </summary>
    public class LeaveCategory
    {
        public int LeaveCategoryId { get; set; }
        public int SortingOrder { get; set; }
        public string LeaveCategoryName { get; set; }
    }
}
