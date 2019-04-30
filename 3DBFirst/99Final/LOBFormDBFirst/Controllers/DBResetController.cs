using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LOBFormDBFirst.Controllers
{
    public class DBResetController : ApiController
    {
        private LOBFormEntities db = new LOBFormEntities();
        public int sortId { get; set; }
        public APIResult Get()
        {
            CleanDB();
            InitialDB();
            return new APIResult()
            {
                Success = true,
                TokenFail = false,
                Message = "資料庫已經清空，預設資料已經重新建立",
                Payload = null
            };
        }

        /// <summary>
        /// 清空資料庫內練習用的所有紀錄
        /// </summary>
        private void CleanDB()
        {
            // 清空使用者紀錄
            db.LOBMyUsers.RemoveRange(db.LOBMyUsers);
            // 清空請假單紀錄
            db.LOBLeaveAppForms.RemoveRange(db.LOBLeaveAppForms);
            // 清空請假單分類
            db.LOBLeaveCategories.RemoveRange(db.LOBLeaveCategories);
            // 清空專案名稱
            db.LOBProjects.RemoveRange(db.LOBProjects);
            // 清空工作紀錄
            db.LOBWorkingLogs.RemoveRange(db.LOBWorkingLogs);
            // 清空On-Call 電話
            db.LOBOnCallPhones.RemoveRange(db.LOBOnCallPhones);
            db.SaveChanges();
        }

        /// <summary>
        /// 準備要練習用的預設資料
        /// </summary>
        private void InitialDB()
        {
            InitProjects();
            InitLeaveCategories();
            InitOnCallPhones();
            InitMyUsers();
            InitLeaveAppForms();
        }

        public void InitProjects()
        {
            #region 專案資料初始化
            for (int i = 0; i < 30; i++)
            {
                db.LOBProjects.Add(new LOBProjects()
                {
                    ProjectName = $"專案{i}"
                });
            }
            db.SaveChanges();
            #endregion
        }

        public void InitLeaveCategories()
        {
            #region 請假類別資料初始化
            sortId = 0;
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "病假(一般病假)"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "病假(住院病假)"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "特別休假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "事假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "事假(家庭照顧假)"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "生理假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "婚假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "喪假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "公假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "公傷病假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "產假含例假日"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "安胎假"
            });
            db.LOBLeaveCategories.Add(new LOBLeaveCategories()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "撫育假"
            });
            db.SaveChanges();
            #endregion
        }

        public void InitOnCallPhones()
        {
            #region 緊急聯絡電話資料初始化
            sortId = 0;
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "警衛室",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "行政部 Administrative Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "稽核室 Auditorial Room",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "董事長室 Chairman's Office",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "電腦中心 Computer Center",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "客戶服務部 Customer Service Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "財務部 Finance Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "管理部 Financial & Administrative Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "總務部 General Affairs Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "人力資源部 Human Resources Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "資訊部 IT Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "行銷部 Marketing Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "企劃部 Planning Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "採購部 Procurement Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "品管部 Quality Control Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "研究開發部 Research & Development Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.LOBOnCallPhones.Add(new LOBOnCallPhones()
            {
                Title = "業務部 Sales Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            db.SaveChanges();
            #endregion
        }

        public void InitMyUsers()
        {
            #region 使用者資料初始化
            sortId = 0;
            for (int i = 0; i < 10; i++)
            {
                var fooManager = new LOBMyUsers()
                {
                    DepartmentName = $"部門{i}",
                    Name = $"經理{i}",
                    EmployeeID = $"manager{i}",
                    Password = $"pwd{i}",
                    ManagerId = -1,
                    IsManager = true
                };
                db.LOBMyUsers.Add(fooManager);
                db.SaveChanges();
                var fooMan = $"manager{i}";
                fooManager = db.LOBMyUsers.FirstOrDefault(x => x.EmployeeID == fooMan);
                for (int j = 1 + (i * 10); j < 11 + (i * 10); j++)
                {
                    db.LOBMyUsers.Add(new LOBMyUsers()
                    {
                        DepartmentName = $"部門{i}",
                        Name = $"使用者{j}",
                        EmployeeID = $"user{j}",
                        Password = $"pwd{j}",
                        ManagerId = fooManager.MyUserId,
                        IsManager = false
                    });
                    db.SaveChanges();
                    var fooU = $"user{j}";
                    var fooUser = db.LOBMyUsers.FirstOrDefault(x => x.EmployeeID == fooU);

                    #region 產生預設的請假單練習資料
                    for (int k = 0; k < 3; k++)
                    {
                        var fooBeginDate = DateTime.Now.AddDays(1).Date.AddHours(9);
                        var fooCompleteDate = DateTime.Now.AddDays(1).Date.AddHours(18);
                        var fooLeaveAppFormItem = new LOBLeaveAppForms()
                        {
                            AgentName = $"我的好友{k}",
                            ApproveResult = "尚未審核",
                            BeginDate = fooBeginDate,
                            Category = "特別休假",
                            CompleteDate = fooCompleteDate,
                            FormDate = DateTime.Now,
                            FormsStatus = "已經送出",
                            LeaveCause = "休息一下",
                            LOBMyUsers = fooUser,
                            Hours = 8
                        };
                        db.LOBLeaveAppForms.Add(fooLeaveAppFormItem);
                    }
                    #endregion

                    #region 產生預設的工作日誌紀錄
                    for (int k = 0; k < 3; k++)
                    {
                        var fooProjectName = $"專案{k}";
                        var fooProject = db.LOBProjects.FirstOrDefault(x => x.ProjectName == fooProjectName);
                        var fooBeginDate = DateTime.Now.AddDays(1).Date.AddHours(9);
                        var fooCompleteDate = DateTime.Now.AddDays(1).Date.AddHours(18);
                        var fooWorkingLog = new LOBWorkingLogs()
                        {
                            LogDate = DateTime.Now,
                            LOBProjects = fooProject,
                            Summary = "討論上線相關準備工作",
                            Title = "與客戶開會",
                            LOBMyUsers = fooUser,
                            Hours = 3
                        };
                        db.LOBWorkingLogs.Add(fooWorkingLog);
                    }
                    #endregion
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
            #endregion

        }

        public void InitLeaveAppForms()
        {
            #region 測試用的請假單
            var fooUser1 = db.LOBMyUsers.FirstOrDefault(x => x.EmployeeID == "user1");
            var fooLeaveAppForm = new LOBLeaveAppForms()
            {
                AgentName = "Vulcan",
                ApproveResult = "Wait",
                BeginDate = DateTime.Now,
                Category = "XX",
                CompleteDate = DateTime.Now,
                FormDate = DateTime.Now,
                FormsStatus = "@@",
                LeaveCause = "??",
                LOBMyUsers = fooUser1,
                //OwnerId = fooUser1.MyUserId,
                Hours = 8,
            };
            db.LOBLeaveAppForms.Add(fooLeaveAppForm);
            db.SaveChanges();
            #endregion
        }

        public void Other()
        {
            int sortId = 0;
            #region 各資料表需要進行初始化的方法


            #region 使用者資料初始化
            sortId = 0;
            for (int i = 0; i < 10; i++)
            {
                var fooManager = new LOBMyUsers()
                {
                    DepartmentName = $"部門{i}",
                    Name = $"經理{i}",
                    EmployeeID = $"manager{i}",
                    Password = $"pwd{i}",
                    ManagerId = -1,
                    IsManager = true
                };
                db.LOBMyUsers.Add(fooManager);
                db.SaveChanges();
                var fooMan = $"manager{i}";
                fooManager = db.LOBMyUsers.FirstOrDefault(x => x.EmployeeID == fooMan);
                for (int j = 1 + (i * 10); j < 11 + (i * 10); j++)
                {
                    db.LOBMyUsers.Add(new LOBMyUsers()
                    {
                        DepartmentName = $"部門{i}",
                        Name = $"使用者{j}",
                        EmployeeID = $"user{j}",
                        Password = $"pwd{j}",
                        ManagerId = fooManager.MyUserId,
                        IsManager = false
                    });
                    db.SaveChanges();
                    var fooU = $"user{j}";
                    var fooUser = db.LOBMyUsers.FirstOrDefault(x => x.EmployeeID == fooU);

                    #region 產生預設的請假單練習資料
                    for (int k = 0; k < 3; k++)
                    {
                        var fooBeginDate = DateTime.Now.AddDays(1).Date.AddHours(9);
                        var fooCompleteDate = DateTime.Now.AddDays(1).Date.AddHours(18);
                        var fooLeaveAppFormItem = new LOBLeaveAppForms()
                        {
                            AgentName = $"我的好友{k}",
                            ApproveResult = "尚未審核",
                            BeginDate = fooBeginDate,
                            Category = "特別休假",
                            CompleteDate = fooCompleteDate,
                            FormDate = DateTime.Now,
                            FormsStatus = "已經送出",
                            LeaveCause = "休息一下",
                            LOBMyUsers = fooUser,
                            Hours = 8
                        };
                        db.LOBLeaveAppForms.Add(fooLeaveAppFormItem);
                    }
                    #endregion

                    #region 產生預設的工作日誌紀錄
                    for (int k = 0; k < 3; k++)
                    {
                        var fooProjectName = $"專案{k}";
                        var fooProject = db.LOBProjects.FirstOrDefault(x => x.ProjectName == fooProjectName);
                        var fooBeginDate = DateTime.Now.AddDays(1).Date.AddHours(9);
                        var fooCompleteDate = DateTime.Now.AddDays(1).Date.AddHours(18);
                        var fooWorkingLog = new LOBWorkingLogs()
                        {
                            LogDate = DateTime.Now,
                            LOBProjects = fooProject,
                            Summary = "討論上線相關準備工作",
                            Title = "與客戶開會",
                            LOBMyUsers = fooUser,
                            Hours = 3
                        };
                        db.LOBWorkingLogs.Add(fooWorkingLog);
                    }
                    #endregion
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
            #endregion

            #endregion
        }
    }
}
