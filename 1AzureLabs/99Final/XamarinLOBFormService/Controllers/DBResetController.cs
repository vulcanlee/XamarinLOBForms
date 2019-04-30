using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XamarinLOBFormService.Models;
using System.Linq;

namespace XamarinLOBFormService.Controllers
{
    [Microsoft.Azure.Mobile.Server.Config.MobileAppController]
    public class DBResetController : ApiController
    {
        private XamarinLOBFormContext Context = new XamarinLOBFormContext();
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
            Context.MyUsers.RemoveRange(Context.MyUsers);
            // 清空請假單紀錄
            Context.LeaveAppForms.RemoveRange(Context.LeaveAppForms);
            // 清空請假單分類
            Context.LeaveCategories.RemoveRange(Context.LeaveCategories);
            // 清空專案名稱
            Context.Projects.RemoveRange(Context.Projects);
            // 清空工作紀錄
            Context.WorkingLogs.RemoveRange(Context.WorkingLogs);
            // 清空On-Call 電話
            Context.OnCallPhones.RemoveRange(Context.OnCallPhones);
            Context.SaveChanges();
        }

        /// <summary>
        /// 準備要練習用的預設資料
        /// </summary>
        private void InitialDB()
        {
            int sortId = 0;
            #region 各資料表需要進行初始化的方法

            #region 請假類別資料初始化
            sortId = 0;
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "病假(一般病假)"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "病假(住院病假)"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "特別休假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "事假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "事假(家庭照顧假)"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "生理假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "婚假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "喪假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "公假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "公傷病假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "產假含例假日"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "安胎假"
            });
            Context.LeaveCategories.Add(new DataObjects.LeaveCategory()
            {
                SortingOrder = sortId++,
                LeaveCategoryName = "撫育假"
            });
            Context.SaveChanges();
            #endregion

            #region 專案資料初始化
            for (int i = 0; i < 30; i++)
            {
                Context.Projects.Add(new DataObjects.Project()
                {
                    ProjectName = $"專案{i}"
                });
            }
            Context.SaveChanges();
            #endregion

            #region 緊急聯絡電話資料初始化
            sortId = 0;
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "警衛室",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "行政部 Administrative Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "稽核室 Auditorial Room",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "董事長室 Chairman's Office",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "電腦中心 Computer Center",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "客戶服務部 Customer Service Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "財務部 Finance Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "管理部 Financial & Administrative Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "總務部 General Affairs Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "人力資源部 Human Resources Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "資訊部 IT Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "行銷部 Marketing Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "企劃部 Planning Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "採購部 Procurement Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "品管部 Quality Control Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "研究開發部 Research & Development Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.OnCallPhones.Add(new DataObjects.OnCallPhone()
            {
                Title = "業務部 Sales Department",
                PhoneNumber = "+886091234567" + (sortId % 10).ToString(),
                SortingOrder = sortId++,
            });
            Context.SaveChanges();
            #endregion

            #region 使用者資料初始化
            sortId = 0;
            for (int i = 0; i < 10; i++)
            {
                var fooManager = new DataObjects.MyUser()
                {
                    DepartmentName = $"部門{i}",
                    Name = $"經理{i}",
                    EmployeeID = $"manager{i}",
                    Password = $"pwd{i}",
                    ManagerId = -1,
                    IsManager = true
                };
                Context.MyUsers.Add(fooManager);
                Context.SaveChanges();
                var fooMan = $"manager{i}";
                fooManager = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == fooMan);
                for (int j = 1 + (i * 10); j < 11 + (i * 10); j++)
                {
                    Context.MyUsers.Add(new DataObjects.MyUser()
                    {
                        DepartmentName = $"部門{i}",
                        Name = $"使用者{j}",
                        EmployeeID = $"user{j}",
                        Password = $"pwd{j}",
                        ManagerId = fooManager.MyUserId,
                        IsManager = false
                    });
                    Context.SaveChanges();
                    var fooU = $"user{j}";
                    var fooUser = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == fooU);

                    #region 產生預設的請假單練習資料
                    for (int k = 0; k < 3; k++)
                    {
                        var fooBeginDate = DateTime.Now.AddDays(1).Date.AddHours(9);
                        var fooCompleteDate = DateTime.Now.AddDays(1).Date.AddHours(18);
                        var fooLeaveAppFormItem = new DataObjects.LeaveAppForm()
                        {
                            AgentName = $"我的好友{k}",
                            ApproveResult = "尚未審核",
                            BeginDate = fooBeginDate,
                            Category = "特別休假",
                            CompleteDate = fooCompleteDate,
                            FormDate = DateTime.Now,
                            FormsStatus = "已經送出",
                            LeaveCause = "休息一下",
                            Owner = fooUser,
                            Hours = 8
                        };
                        Context.LeaveAppForms.Add(fooLeaveAppFormItem);
                    }
                    #endregion

                    #region 產生預設的工作日誌紀錄
                    for (int k = 0; k < 3; k++)
                    {
                        var fooProjectName = $"專案{k}";
                        var fooProject = Context.Projects.FirstOrDefault(x => x.ProjectName == fooProjectName);
                        var fooBeginDate = DateTime.Now.AddDays(1).Date.AddHours(9);
                        var fooCompleteDate = DateTime.Now.AddDays(1).Date.AddHours(18);
                        var fooWorkingLog = new DataObjects.WorkingLog()
                        {
                             LogDate = DateTime.Now,
                            Project = fooProject,
                            Summary = "討論上線相關準備工作",
                                Title = "與客戶開會",
                            Owner = fooUser,
                            Hours = 3
                        };
                        Context.WorkingLogs.Add(fooWorkingLog);
                    }
                    #endregion
                    Context.SaveChanges();
                }
            }
            Context.SaveChanges();
            #endregion

            #region 測試用的請假單
            var fooUser1 = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == "user1");
            var fooLeaveAppForm = new DataObjects.LeaveAppForm()
            {
                AgentName = "Vulcan",
                ApproveResult = "Wait",
                BeginDate = DateTime.Now,
                Category = "XX",
                CompleteDate = DateTime.Now,
                FormDate = DateTime.Now,
                FormsStatus = "@@",
                LeaveCause = "??",
                Owner = fooUser1,
                //OwnerId = fooUser1.MyUserId,
                Hours = 8,
            };
            Context.LeaveAppForms.Add(fooLeaveAppForm);
            Context.SaveChanges();

            var fooList = Context.LeaveAppForms.ToList();
            #endregion
            #endregion
        }

    }
}
