using AuthToken.Helpers;
using AuthToken.Models;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace AuthToken.Controllers
{
    public class LoginController : ApiController
    {
        HttpResponseMessage response;
        public string Account { get; set; }
        public string Password { get; set; }
        [HttpGet]
        public HttpResponseMessage Get()
        {
            if (CanHandleAuthentication(this.Request) == true)
            {
                #region 檢查帳號與密碼是否正確
                // 這裡可以修改成為與後端資料庫內的使用者資料表進行比對
                var expectAccount = "vulcan";
                var expectPassword = "123abc";
                if (expectAccount == Account &&
                    expectPassword == Password)
                {
                    #region 產生這次通過身分驗證的存取權杖 Access Token
                    string secretKey = MainHelper.SecretKey;
                    #region 設定該存取權杖的有效期限
                    IDateTimeProvider provider = new UtcDateTimeProvider();
                    // 這個 Access Token只有一個小時有效
                    var now = provider.GetNow().AddHours(1);
                    var unixEpoch = UnixEpoch.Value; // 1970-01-01 00:00:00 UTC
                    var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);
                    #endregion

                    var jwtToken = new JwtBuilder()
                          .WithAlgorithm(new HMACSHA256Algorithm())
                          .WithSecret(secretKey)
                          .AddClaim("iss", Account)
                          .AddClaim("exp", secondsSinceEpoch)
                          .AddClaim("role", new string[] { "Manager", "People" })
                          .Build();
                    #endregion

                    // 帳號與密碼比對正確，回傳帳密比對正確
                    response = this.Request.CreateResponse<APIResult>(HttpStatusCode.OK, new APIResult()
                    {
                        Success = true,
                        Message = $"帳號:{Account} / 密碼:{Password}",
                        Payload = $"{jwtToken}"
                    });
                }
                else
                {
                    // 帳號與密碼比對不正確，回傳帳密比對不正確
                    response = this.Request.CreateResponse<APIResult>(HttpStatusCode.Unauthorized, new APIResult()
                    {
                        Success = false,
                        Message = $"",
                        Payload = "帳號或密碼不正確"
                    });
                }
                #endregion
                return response;
            }
            else
            {
                // 沒有收到正確格式的 Authorization 內容，回傳無法驗證訊息
                response = this.Request.CreateResponse<APIResult>(HttpStatusCode.Unauthorized, new APIResult()
                {
                    Success = false,
                    Message = $"",
                    Payload = "沒有收到帳號與密碼"
                });
                return response;
            }
        }

        /// <summary>
        /// 檢查與解析 Authorization 標頭是否存在與解析用戶端傳送過來的帳號與密碼
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanHandleAuthentication(HttpRequestMessage request)
        {
            // 驗證結果是否正確
            bool isSuccess = false;

            #region 檢查是否有使用 Authorization: Basic 傳送帳號與密碼到 Web API 伺服器
            if ((request.Headers != null
                    && request.Headers.Authorization != null
                    && request.Headers.Authorization.Scheme.ToLowerInvariant() == "basic"))
            {
                #region 取出帳號與密碼，帳號與密碼格式為 帳號:密碼
                var authHeader = request.Headers.Authorization;

                // 取出有 Base64 編碼的帳號與密碼
                var encodedCredentials = authHeader.Parameter;
                // 進行 Base64 解碼
                var credentialBytes = Convert.FromBase64String(encodedCredentials);
                // 取得 .NET 字串
                var credentials = Encoding.ASCII.GetString(credentialBytes);
                // 判斷格式是否正確
                var credentialParts = credentials.Split(':');

                if (credentialParts.Length == 2)
                {
                    // 取出使用者傳送過來的帳號與密碼
                    Account = credentialParts[0];
                    Password = credentialParts[1];
                    isSuccess = true;
                }

                #endregion
            }
            #endregion
            return isSuccess;
        }
    }
}
