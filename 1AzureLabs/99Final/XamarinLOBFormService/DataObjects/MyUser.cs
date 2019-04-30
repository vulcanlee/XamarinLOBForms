using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XamarinLOBFormService.DataObjects
{
    /// <summary>
    /// 使用者
    /// </summary>
    public class MyUser 
    {
        public int MyUserId { get; set; }
        public string DepartmentName { get; set; }
        public string Name { get; set; }
        public string EmployeeID { get; set; }
        public string Password { get; set; }
        //public MyUser Manager { get; set; }
        public int ManagerId { get; set; }
        public bool IsManager { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}