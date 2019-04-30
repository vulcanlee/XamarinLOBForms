using Acr.UserDialogs;
using LOBForm.Models;
using LOBForm.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LOBForm.Helpers
{
    /// <summary>
    /// 這是整個應用程式都可以存取的一個支援類別屬性與方法
    /// </summary>
    public class MainHelper
    {
        #region 常用的變數字串
        #region 向 Azure Mobile App 服務的主要網址
        public const string MainURL = "http://xamarinlobform.azurewebsites.net/";
        #endregion

        #region 呼叫 API 的最上層名稱
        public static string BaseAPIUrl = $"{MainURL}api/";
        #endregion

        #region 使用者身分驗證的 API 名稱
        public static string UserLoginAPIName = $"Login";
        public static string UserLoginAPIUrl = $"{BaseAPIUrl}{UserLoginAPIName}";
        #endregion

        #region 專案清單所有紀錄的 API 名稱
        public static string ProjectAPIName = $"Project";
        public static string ProjectAPIUrl = $"{BaseAPIUrl}{ProjectAPIName}";
        #endregion

        #region 請假類別清單所有紀錄的 API 名稱
        public static string LeaveCategoryAPIName = $"LeaveCategory";
        public static string LeaveCategoryAPIUrl = $"{BaseAPIUrl}{LeaveCategoryAPIName}";
        #endregion

        #region 使用者部門清單所有紀錄的 API 名稱
        public static string DepartmentAPIName = $"Department";
        public static string DepartmentAPIUrl = $"{BaseAPIUrl}{DepartmentAPIName}";
        #endregion

        #region 請假單代理人清單查詢 API 名稱
        public static string LeaveAppFormAgentAPIName = $"QueryAgent";
        public static string LeaveAppFormAgentAPIUrl = $"{BaseAPIUrl}{LeaveAppFormAgentAPIName}";
        #endregion

        #region On-Call清單所有紀錄的 API 名稱
        public static string OnCallPhoneAPIName = $"OnCallPhone";
        public static string OnCallPhoneAPIUrl = $"{BaseAPIUrl}{OnCallPhoneAPIName}";
        #endregion

        #region 工作日誌清單所有紀錄的 API 名稱
        public static string WorkingLogByUserIDAPIName = $"WorkingLog/ByUserID";
        public static string WorkingLogAPIName = $"WorkingLog";
        public static string WorkingLogByUserIDAPIUrl = $"{BaseAPIUrl}{WorkingLogByUserIDAPIName}";
        public static string WorkingLogAPIUrl = $"{BaseAPIUrl}{WorkingLogAPIName}";
        #endregion

        #region 請假類清單所有紀錄的 API 名稱
        public static string LeaveAppFormManagerMode = $"manager";
        public static string LeaveAppFormUserMode = $"user";
        public static string LeaveAppFormByUserIDAPIName = $"LeaveAppFormByUser";
        public static string LeaveAppFormAPIName = $"LeaveAppForm";
        public static string LeaveAppFormByUserIDAPIUrl = $"{BaseAPIUrl}{LeaveAppFormByUserIDAPIName}";
        public static string LeaveAppFormAPIUrl = $"{BaseAPIUrl}{LeaveAppFormAPIName}";

        #endregion

        #region 系統運作狀態的存取檔案名稱
        public static string SystemStatusFileName = $"SystemStatus";
        #endregion

        #region 寫入裝置內的檔案所在的主目錄名稱
        public static string 資料主目錄 = $"Data";
        #endregion

        #endregion

        #region 其他常用字串
        public static string Prism__NavigationMode { get; set; } = "__NavigationMode";
        public static string CRUDItemKeyName { get; set; } = "CRUDItem";
        public static string CRUDKeyName { get; set; } = "CRUDMode";
        public static string CRUDFromDetailKeyName { get; set; } = "DetailAction";
        public static string CRUD_Create { get; set; } = "Create";
        public static string CRUD_Update { get; set; } = "Update";
        public static string CRUD_Delete { get; set; } = "Delete";
        public static string QueryUserAgent { get; set; } = "QueryUserAgent";
        #endregion

        #region Repository (此處為方便開發，所以，所有的 Repository 皆為全域靜態可存取)
        #endregion

        /// <summary>
        /// 要進行登出，所以，清空本機快取資料
        /// </summary>
        /// <returns></returns>
        public static async Task CleanRepositories()
        {
            #region 要進行登出，所以，清空本機快取資料
            var fooSystemStatusRepository = new SystemStatusRepository();
            await fooSystemStatusRepository.ReadAsync();
            fooSystemStatusRepository.Item.AccessToken = "";
            await fooSystemStatusRepository.SaveAsync();

            var fooLoginRepository = new LoginRepository();
            fooLoginRepository.Item = new Models.UserLoginResultModel();
            await fooLoginRepository.SaveAsync();

            var fooWorkingLogRepository = new WorkingLogRepository();
            fooWorkingLogRepository.Items = new List<Models.WorkingLog>();
            await fooWorkingLogRepository.SaveAsync();

            var fooLeaveAppFormRepository = new LeaveAppFormRepository();
            fooLeaveAppFormRepository.Items = new List<Models.LeaveAppForm>();
            await fooLeaveAppFormRepository.SaveAsync(MainHelper.LeaveAppFormUserMode);
            await fooLeaveAppFormRepository.SaveAsync(MainHelper.LeaveAppFormManagerMode);
            #endregion

        }

        /// <summary>
        /// 若存取權杖 Access Token 發生了問題，需要進行的檢查與處理動作
        /// </summary>
        /// <param name="apiResult"></param>
        /// <returns></returns>
        public static async Task<bool> CheckAccessToken(APIResult apiResult)
        {
            bool IsValid = true;
            #region 若存取權杖 Access Token 發生了問題，需要進行的檢查與處理動作
            if (apiResult.TokenFail == true)
            {
                try
                {
                    var fooAlertConfig = new AlertConfig()
                    {
                        Title = "錯誤通知",
                        Message = $"存取權杖異常，請登出後，進行重新登入驗證作業",
                        OkText = "確定"
                    };
                    CancellationTokenSource fooCancelSrc = new CancellationTokenSource(10000);
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(fooAlertConfig, fooCancelSrc.Token);
                }
                catch (OperationCanceledException)
                {
                }

                IsValid = false;
            }
            #endregion
            return IsValid;
        }
    }
}
