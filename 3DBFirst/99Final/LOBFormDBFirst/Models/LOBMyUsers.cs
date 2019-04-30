using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOBFormDBFirst
{
    public partial class LOBMyUsers
    {
        public MyUser ToMyUsers()
        {
            return new MyUser()
            {
                MyUserId = this.MyUserId,
                DepartmentName = this.DepartmentName,
                EmployeeID = this.EmployeeID,
                IsManager = this.IsManager,
                ManagerId = this.ManagerId,
                Name = this.Name,
                Password = this.Password,
            };
        }
    }
}