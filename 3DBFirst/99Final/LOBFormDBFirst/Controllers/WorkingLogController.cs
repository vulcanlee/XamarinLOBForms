using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LOBFormDBFirst.Controllers
{
#if !DEBUG
    [Filters.JwtAuth]
#endif

    [RoutePrefix("api/WorkingLog")]
    public class WorkingLogController : ApiController
    {
        private LOBFormEntities db = new LOBFormEntities();
        private APIResult fooResult = new APIResult();

        // 取得所有工作日誌資料 GET: api/WorkingLog
        [HttpGet]
        public async Task<APIResult> Get()
        {
            fooResult.Success = true;
            fooResult.Message = $"";
            fooResult.TokenFail = false;
            fooResult.Payload = (await db.LOBWorkingLogs.ToListAsync()).Select(x => x.ToWorkingLog());
            return fooResult;
        }

        //查詢某筆工作日誌資料 GET: api/WorkingLog/5
        [HttpGet]
        public async Task<APIResult> Get(int id)
        {
            var fooItem = (await db.LOBWorkingLogs.FirstOrDefaultAsync(x => x.WorkingLogId == id));
            if (fooItem != null)
            {
                fooResult.Success = true;
                fooResult.Message = $"";
                fooResult.TokenFail = false;
                fooResult.Payload = fooItem.ToWorkingLog();
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"指定的紀錄編號，無法找到符合資料";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            return fooResult;
        }

        //查詢指定使用者的所有工作日誌紀錄 GET: api/LeaveAppForm/ByUserID? userID = User1 & mode = NA
        [HttpGet]
        [Route("ByUserID")]
        public async Task<APIResult> GetByUserID(string userID, string mode)
        {
            #region 取得特定使用者的所有工作日誌
            var fooUser = await db.LOBMyUsers.FirstOrDefaultAsync(x => x.EmployeeID == userID);
            if (fooUser != null)
            {
                var fooItem = db.LOBWorkingLogs.Where(x => x.LOBMyUsers.MyUserId == fooUser.MyUserId);
                if (fooItem != null)
                {
                    fooResult.Success = true;
                    fooResult.Message = $"";
                    fooResult.TokenFail = false;
                    fooResult.Payload = (await fooItem.ToListAsync()).Select(x => x.ToWorkingLog());
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
        public async Task<APIResult> Post([FromBody]WorkingLog value)
        {
            #region 檢查使用者是否存在
            var fooUser = await db.LOBMyUsers.FirstOrDefaultAsync(x => x.EmployeeID == value.Owner.EmployeeID);
            if (fooUser != null)
            {
                #region 檢查使用者是否存在
                var fooProject = await db.LOBProjects.FirstOrDefaultAsync(x => x.ProjectId == value.Project.ProjectId);
                if (fooProject != null)
                {
                    #region 產生工作日誌物件
                    var fooLeaveAppFormItem = new LOBWorkingLogs()
                    {
                        LogDate = value.LogDate,
                        Summary = value.Summary,
                        Title = value.Title,
                        Hours = value.Hours,
                        LOBMyUsers = fooUser,
                        LOBProjects = fooProject

                    };
                    db.LOBWorkingLogs.Add(fooLeaveAppFormItem);
                    #endregion
                    var fooCC = db.SaveChanges();
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
        public async Task<APIResult> Put([FromBody]WorkingLog value)
        {
            #region 檢查使用者是否存在
            var fooUser = await db.LOBMyUsers.FirstOrDefaultAsync(x => x.EmployeeID == value.Owner.EmployeeID);
            if (fooUser != null)
            {
                #region 檢查使用者是否存在
                var fooProject = await db.LOBProjects.FirstOrDefaultAsync(x => x.ProjectId == value.Project.ProjectId);
                if (fooProject != null)
                {
                    var fooItem = await db.LOBWorkingLogs.FirstOrDefaultAsync(x => x.WorkingLogId == value.WorkingLogId);
                    if (fooItem != null)
                    {
                        #region 更新資料庫上的紀錄
                        fooItem.LogDate = value.LogDate;
                        fooItem.LOBMyUsers = fooUser;
                        fooItem.LOBProjects = fooProject;
                        fooItem.Summary = value.Summary;
                        fooItem.Title = value.Title;
                        fooItem.Hours = value.Hours;
                        #endregion
                        var fooCC = db.SaveChanges();
                        if (fooCC > 0)
                        {
                            fooResult.Success = true;
                            fooResult.Message = $"工作日誌紀錄({fooItem.WorkingLogId})成功被修改";
                            fooResult.TokenFail = false;
                            fooResult.Payload = fooItem.ToWorkingLog();
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
        public async Task<APIResult> Delete(int id)
        {
            var fooItem = await db.LOBWorkingLogs.FirstOrDefaultAsync(x => x.WorkingLogId == id);
            if (fooItem != null)
            {
                db.LOBWorkingLogs.Remove(fooItem);
                var fooCC = db.SaveChanges();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
