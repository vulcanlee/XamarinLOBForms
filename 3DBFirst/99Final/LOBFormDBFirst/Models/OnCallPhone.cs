using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst.Models
{
    /// <summary>
    /// 公司緊急連絡方式
    /// </summary>
    public class OnCallPhone
    {
        public int OnCallPhoneId { get; set; }
        public int SortingOrder { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
    }
}