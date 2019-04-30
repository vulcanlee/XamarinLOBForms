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
    [Filters.JwtAuth]
    public class LeaveAppFormByUserController : ApiController
    {
        private XamarinLOBFormContext Context = new XamarinLOBFormContext();
        APIResult fooResult = new APIResult();

        // 查詢特定主管要審核或指定使用者的所有請假單紀錄 Post: api/LeaveAppFormByUser
        [HttpPost]
        //public APIResult GetByUserID(string userID, string mode)
        public APIResult Post([FromBody]LeaveAppFormByUserModel leaveAppFormByUserModel)
        {
            var mode = leaveAppFormByUserModel.Mode;
            var userID = leaveAppFormByUserModel.Account;
            if (mode.ToLower() == "manager")
            {
                #region 取得該主管需要審核的請假單
                var fooManageUser = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == userID);
                if (fooManageUser != null)
                {
                    #region 檢查該使用者是否為管理者
                    if (fooManageUser.IsManager == false)
                    {
                        fooResult.Success = false;
                        fooResult.Message = $"這個使用者 {userID} 不具備主管身分";
                        fooResult.TokenFail = false;
                        fooResult.Payload = null;
                        return fooResult;
                    }
                    #endregion
                    var fooItem = Context.LeaveAppForms.Where(x => x.Owner.ManagerId == fooManageUser.MyUserId);
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
                    fooResult.Message = $"找不到指定主管資料";
                    fooResult.TokenFail = false;
                    fooResult.Payload = null;
                }
                #endregion
            }
            else
            {
                #region 取得特定使用者的所有請假單
                var fooUser = Context.MyUsers.FirstOrDefault(x => x.EmployeeID == userID);
                if (fooUser != null)
                {
                    var fooItem = Context.LeaveAppForms.Where(x => x.Owner.MyUserId == fooUser.MyUserId);
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
            }
            return fooResult;
        }

    }
}
