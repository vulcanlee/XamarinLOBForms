using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XamarinLOBFormService.DataObjects;
using XamarinLOBFormService.Models;

namespace XamarinLOBFormService.Controllers
{
    [Microsoft.Azure.Mobile.Server.Config.MobileAppController]
    //[Filters.JwtAuth]
    public class WorkingLogController : ApiController
    {
        private XamarinLOBFormContext Context = new XamarinLOBFormContext();
        APIResult fooResult = new APIResult();

        // 取得所有工作日誌資料 GET: api/WorkingLog
        public APIResult Get()
        {
            fooResult.Success = true;
            fooResult.Message = $"";
            fooResult.TokenFail = false;
            fooResult.Payload = Context.WorkingLogs.ToList();
            return fooResult;
        }

        // 查詢某筆工作日誌資料 GET: api/WorkingLog/5
        public APIResult Get(int id)
        {
            var fooItem = Context.WorkingLogs.FirstOrDefault(x => x.WorkingLogId == id);
            if (fooItem != null)
            {
                fooResult.Success = true;
                fooResult.Message = $"";
                fooResult.TokenFail = false;
                fooResult.Payload = fooItem;
            }
            else
            {
                fooResult.Success = true;
                fooResult.Message = $"";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            return fooResult;
        }

        // 查詢指定使用者的所有工作日誌紀錄 GET: api/LeaveAppForm/ByUserID?userID=User1&mode=NA
        [HttpGet]
        [Route("ByUserID")]
        public APIResult GetByUserID(string userID, string mode)
        {
            #region 取得特定使用者的所有請假單
            var fooUser = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == userID);
            if (fooUser != null)
            {
                var fooItem = Context.WorkingLogs.Where(x => x.Owner.MyUserId == fooUser.MyUserId);
                if (fooItem != null)
                {
                    fooResult.Success = true;
                    fooResult.Message = $"";
                    fooResult.TokenFail = false;
                    fooResult.Payload = fooItem.ToList();
                }
                else
                {
                    fooResult.Success = true;
                    fooResult.Message = $"";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"找不到指定使用者資料";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            #endregion
            return fooResult;
        }

        // 新增資料 POST: api/LeaveAppForm
        public APIResult Post([FromBody]WorkingLog value)
        {
            #region 檢查使用者是否存在
            var fooUser = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == value.Owner.EmployeeID);
            if (fooUser != null)
            {
                #region 檢查使用者是否存在
                var fooProject = Context.Projects.FirstOrDefault(x => x.ProjectId == value.Project.ProjectId);
                if (fooProject != null)
                {
                    #region 產生工作日誌物件
                    var fooLeaveAppFormItem = new WorkingLog()
                    {
                        LogDate = value.LogDate,
                        Summary = value.Summary,
                        Title = value.Title,
                        Hours = value.Hours,
                        Owner = fooUser,
                        Project = fooProject

                    };
                    Context.WorkingLogs.Add(fooLeaveAppFormItem);
                    #endregion
                    var fooCC = Context.SaveChanges();
                    if (fooCC > 0)
                    {
                        fooResult.Success = true;
                        fooResult.Message = $"工作日誌紀錄新增成功";
                        fooResult.TokenFail = false;
                        fooResult.Payload = null;
                    }
                    else
                    {
                        fooResult.Success = false;
                        fooResult.Message = $"無法新增到這筆工作日誌紀錄";
                        fooResult.TokenFail = false;
                        fooResult.Payload = null;
                    }
                }
                else
                {
                    fooResult.Success = false;
                    fooResult.Message = $"這筆工作日誌紀錄指定的專案不存在";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
                #endregion
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"這筆工作日誌紀錄指定的使用者不存在";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            #endregion
            return fooResult;
        }

        // 修改資料 PUT: api/LeaveAppForm/5
        public APIResult Put([FromBody]WorkingLog value)
        {
            #region 檢查使用者是否存在
            var fooUser = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == value.Owner.EmployeeID);
            if (fooUser != null)
            {
                #region 檢查使用者是否存在
                var fooProject = Context.Projects.FirstOrDefault(x => x.ProjectId == value.Project.ProjectId);
                if (fooProject != null)
                {
                    var fooItem = Context.WorkingLogs.FirstOrDefault(x => x.WorkingLogId == value.WorkingLogId);
                    if (fooItem != null)
                    {
                        #region 更新資料庫上的紀錄
                        fooItem.LogDate = value.LogDate;
                        fooItem.Owner = fooUser;
                        fooItem.Project = fooProject;
                        fooItem.Summary = value.Summary;
                        fooItem.Title = value.Title;
                        fooItem.Hours = value.Hours;
                        #endregion
                        var fooCC = Context.SaveChanges();
                        if (fooCC > 0)
                        {
                            fooResult.Success = true;
                            fooResult.Message = $"工作日誌紀錄({fooItem.WorkingLogId})成功被修改";
                            fooResult.TokenFail = false;
                            fooResult.Payload = fooItem;
                        }
                        else
                        {
                            fooResult.Success = false;
                            fooResult.Message = $"無法修改這筆 {fooItem.WorkingLogId} 工作日誌紀錄";
                            fooResult.TokenFail = false;
                            fooResult.Payload = null;
                        }
                    }
                }
                else
                {
                    fooResult.Success = false;
                    fooResult.Message = $"這筆工作日誌紀錄指定的專案不存在";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
                #endregion
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"無法發現到這筆 {value.WorkingLogId} 工作日誌錄";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            #endregion
            return fooResult;
        }

        // 刪除資料 DELETE: api/LeaveAppForm/5
        public APIResult Delete(int id)
        {
            var fooItem = Context.WorkingLogs.FirstOrDefault(x => x.WorkingLogId == id);
            if (fooItem != null)
            {
                Context.WorkingLogs.Remove(fooItem);
                var fooCC = Context.SaveChanges();
                if (fooCC > 0)
                {
                    fooResult.Success = true;
                    fooResult.Message = $"工作日誌錄紀錄({id})成功被刪除";
                    fooResult.TokenFail = false;
                    fooResult.Payload = fooItem;
                }
                else
                {
                    fooResult.Success = false;
                    fooResult.Message = $"無法刪除到這筆 {id} 工作日誌錄紀錄";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"無法發現到這筆 {id} 工作日誌錄紀錄";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            return fooResult;
        }
    }
}
