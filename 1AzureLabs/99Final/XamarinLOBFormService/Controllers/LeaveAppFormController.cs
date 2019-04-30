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
    public class LeaveAppFormController : ApiController
    {
        private XamarinLOBFormContext Context = new XamarinLOBFormContext();
        APIResult fooResult = new APIResult();

        // 取得所有資料 GET: api/LeaveAppForm
        public APIResult Get()
        {
            fooResult.Success = true;
            fooResult.Message = $"";
            fooResult.TokenFail = false;
            fooResult.Payload = Context.LeaveAppForms.ToList();
            return fooResult;
        }

        // 查詢某筆資料 GET: api/LeaveAppForm/5
        public APIResult Get(int id)
        {
            var fooItem = Context.LeaveAppForms.FirstOrDefault(x => x.LeaveAppFormId == id);
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

        // 新增資料 POST: api/LeaveAppForm
        public APIResult Post([FromBody]LeaveAppForm value)
        {
            #region 檢查使用者是否存在
            var fooUser = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == value.Owner.EmployeeID);
            if (fooUser != null)
            {
                #region 產生請假單物件
                var fooLeaveAppFormItem = new LeaveAppForm()
                {
                    AgentName = value.AgentName,
                    BeginDate = value.BeginDate,
                    ApproveResult = "尚未審核",
                    Category = value.Category,
                    CompleteDate = value.CompleteDate,
                    FormDate = value.FormDate,
                    FormsStatus = "已經送出",
                    LeaveCause = value.LeaveCause,
                    Owner = fooUser,
                    Hours = value.Hours,
                     AgentId = value.AgentId,
                };
                Context.LeaveAppForms.Add(fooLeaveAppFormItem);
                #endregion
                var fooCC = Context.SaveChanges();
                if (fooCC > 0)
                {
                    fooResult.Success = true;
                    fooResult.Message = $"請假單紀錄新增成功";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
                else
                {
                    fooResult.Success = false;
                    fooResult.Message = $"無法新增到這筆請假單紀錄";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"這筆請假單紀錄指定的使用者不存在";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            #endregion
            return fooResult;
        }

        // 修改資料 PUT: api/LeaveAppForm/5
        public APIResult Put([FromBody]LeaveAppForm value)
        {
            var fooItem = Context.LeaveAppForms.FirstOrDefault(x => x.LeaveAppFormId == value.LeaveAppFormId);
            if (fooItem != null)
            {
                #region 更新資料庫上的紀錄
                fooItem.AgentName = value.AgentName;
                fooItem.ApproveResult = value.ApproveResult;
                fooItem.BeginDate = value.BeginDate;
                fooItem.Category = value.Category;
                fooItem.CompleteDate = value.CompleteDate;
                fooItem.FormDate = value.FormDate;
                fooItem.FormsStatus = value.FormsStatus;
                fooItem.LeaveCause = value.LeaveCause;
                //fooItem.Owner = value.Owner;
                fooItem.Hours = value.Hours;
                fooItem.AgentId = value.AgentId;
                #endregion
                var fooCC = Context.SaveChanges();
                if (fooCC > 0)
                {
                    fooResult.Success = true;
                    fooResult.Message = $"請假單紀錄({fooItem.LeaveAppFormId})成功被修改";
                    fooResult.TokenFail = false;
                    fooResult.Payload = fooItem;
                }
                else
                {
                    fooResult.Success = false;
                    fooResult.Message = $"無法修改這筆 {fooItem.LeaveAppFormId} 請假單紀錄";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"無法發現到這筆 {value.LeaveAppFormId} 請假單紀錄";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            return fooResult;
        }

        // 刪除資料 DELETE: api/LeaveAppForm/5
        public APIResult Delete(int id)
        {
            var fooItem = Context.LeaveAppForms.FirstOrDefault(x => x.LeaveAppFormId == id);
            if (fooItem != null)
            {
                Context.LeaveAppForms.Remove(fooItem);
                var fooCC = Context.SaveChanges();
                if (fooCC > 0)
                {
                    fooResult.Success = true;
                    fooResult.Message = $"請假單紀錄({id})成功被刪除";
                    fooResult.TokenFail = false;
                    fooResult.Payload = fooItem;
                }
                else
                {
                    fooResult.Success = false;
                    fooResult.Message = $"無法刪除到這筆 {id} 請假單紀錄";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"無法發現到這筆 {id} 請假單紀錄";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            return fooResult;
        }
    }
}
