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
    public class LeaveAppFormByUserController : ApiController
    {
        private LOBFormEntities db = new LOBFormEntities();
        private APIResult fooResult = new APIResult();

        // 查詢特定主管要審核或指定使用者的所有請假單紀錄 Post: api/LeaveAppFormByUser
        [HttpPost]
        //public APIResult GetByUserID(string userID, string mode)
        public async Task<APIResult> Post([FromBody]LeaveAppFormByUserModel leaveAppFormByUserModel)
        {
            var mode = leaveAppFormByUserModel.Mode;
            var userID = leaveAppFormByUserModel.Account;
            if (mode.ToLower() == "manager")
            {
                #region 取得該主管需要審核的請假單
                var fooManageUser = await db.LOBMyUsers.FirstOrDefaultAsync(x => x.EmployeeID == userID);
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
                    var fooItem = db.LOBLeaveAppForms.Where(x => x.LOBMyUsers.ManagerId == fooManageUser.MyUserId);
                    if (fooItem != null)
                    {
                        fooResult.Success = true;
                        fooResult.Message = $"";
                        fooResult.TokenFail = false;
                        fooResult.Payload = (await fooItem.ToListAsync()).Select(x => x.ToLeaveAppForm());
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
                var fooUser = await db.LOBMyUsers.FirstOrDefaultAsync(x => x.EmployeeID == userID);
                if (fooUser != null)
                {
                    var fooItem = db.LOBLeaveAppForms.Where(x => x.Owner_MyUserId == fooUser.MyUserId);
                    if (fooItem != null)
                    {
                        fooResult.Success = true;
                        fooResult.Message = $"";
                        fooResult.TokenFail = false;
                        fooResult.Payload = (await fooItem.ToListAsync()).Select(x => x.ToLeaveAppForm());
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
