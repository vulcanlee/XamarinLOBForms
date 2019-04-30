using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XamarinLOBFormService.DataObjects
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