using System;
using System.Collections.Generic;
using System.Text;

namespace LOBForm.Models
{
    /// <summary>
    /// 系統運作狀態
    /// </summary>
    public class SystemStatus
    {
        /// <summary>
        /// Web API 存取權杖
        /// </summary>
        public string AccessToken { get; set; } = "";
        /// <summary>
        /// 使用 Authorization Basic 方式來進行使用者身分驗證
        /// </summary>
        public bool LoginMethodAction { get; set; } = true;
    }
}
