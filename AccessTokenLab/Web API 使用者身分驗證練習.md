# Web API 使用者身分驗證練習

這是一份關於 ASP.NET Web API 如何實作出使用者身分驗證與產生、驗證、使用存取權杖的練習說明文件。

各位學員可以先依照這份文件中的說明步驟，逐一進行練習，了解如何設計出可以產生一個存取權杖的登入服務，並且使用該存取權杖，呼叫其他需要需要有通過使用者驗證才能夠使用的 Web API 服務。

我們會使用 PostMan 這個工具來協助我們進行相關 Web API 的測試工作。

這份文件將使用 Visual Studio 2017 工具來展示如何做到這樣的需求

# 01 建立 ASP.NET Web API 專案

我們首先需要建立一個 ASP.NET Web API (.NET Framework) 專案，接著，從無到有的接著設計出具有存取權杖的身分驗證才能夠存取的 Web API 服務。

* 開啟 Visual Studio 2017 IDE

* 從功能表點選 \[檔案] > \[新增] > \[專案]

* 在 \[新增專案] 對話窗左半部，點選 \[已安裝] > \[Visual C#] > \[Web]

* 在這個對話的中間上方，選擇 \[.NET Framework 4.6.1]

* 接著選取 [ASP.NET Web 應用程式 (.NET Framework)]

* 在下方 \[名稱] 欄位，輸入 `AuthToken`

* 在 \[方案名稱] 欄位，輸入 `AuthTokenSolution`

* 最後，點選右下方的 \[確定] 按鈕

![新增專案](Images/NewProject.png)

* 當出現 \[新增 ASP.NET Web 應用程式 - AuthToken] 對話窗

* 請選擇 \[Web API]

* 右半部的 \[驗證] 請設定為 `無驗證`

* 其他設定可以參考這個螢幕截圖

![AddASPNETWeb](Images/AddASPNETWeb.png)

* 執行這個 Web API 專案，就會看到這個畫面，那就表示您的 Web API 專案已經建立完成

![第一次執行 Web API 專案](Images/FirstStartup.png)

> 這個練習的完成專案，可以參考資料夾 \[01 CreatProject]

# 02 ASP.NET Web API 專案之準備工作

在這裡，我們需要安裝 NuGet 套件與建立會用到的回傳資料模型和基本的登入與兩個 Web API服務。

## 安裝 JWT.NET 套件

* 滑鼠右擊 \[參考] 專案節點，選擇 \[管理 NuGet 套件]

* 在 \[NuGet: AuthToken] 視窗中，點選 \[瀏覽] 標籤頁次

* 在 \[搜尋] 文字輸入盒內，輸入 `JWT.NET`，搜尋這個套件

* 勾選 \[包含搶鮮版] 檢查盒，安裝 4.0 以上的版本

![JWT.NET](Images/NuGet_JWTNET.png)

![NuGet Preview](Images/NuGetPreview.png)

## 建立 Web API 回傳資料模型

* 滑鼠右擊 \[Models] 資料夾，選取 \[加入] > \[類別]

* 在 \[新增項目] 對話窗中，在下方的 \[名稱] 欄位中，輸入 `APIResult`

* 點選 \[新增] 按鈕，完成新增這個類別檔案

* 將這個新產生的類別 `APIResult` 以底下程式碼替換

```cs
/// <summary>
/// 呼叫 API 回傳的制式格式
/// </summary>
public class APIResult
{
    /// <summary>
    /// 此次呼叫 API 是否成功
    /// </summary>
    public bool Success { get; set; } = true;
    /// <summary>
    /// 呼叫 API 失敗的錯誤訊息
    /// </summary>
    public string Message { get; set; } = "";
    /// <summary>
    /// 呼叫此API所得到的其他內容
    /// </summary>
    public object Payload { get; set; }
}
```

## 建立全域常數類別

* 滑鼠右擊專案節點 \[AuthToken]，選擇 \[新增資料夾]

* 在新增的資料夾位置，輸入 `Helpers` 資料夾

* 滑鼠右擊 \[Helpers] 資料夾，選取 \[加入] > \[類別]

* 在 \[新增項目] 對話窗中，在下方的 \[名稱] 欄位中，輸入 `MainHelper`

* 點選 \[新增] 按鈕，完成新增這個類別檔案

* 將這個新產生的類別 `MainHelper` 以底下程式碼替換

```csharp
public class MainHelper
{
    // 這裡是要進行雜湊簽章會用到的金鑰
    public const string SecretKey = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
}
```

## 建立登入驗證 Controllers

* 滑鼠右擊 \[Controllers] 資料夾，選取 \[加入] > \[控制器]

* 在 \[新增 Scaffold] 對話窗中，選擇 \[Web API2 控制器 - 空白]

* 點選右下方的 \[新增] 按鈕

![New Scaffold](Images/NewScaffold.png)

* 當出現 \[加入控制器] 對話窗後，於 \[控制器名稱] 欄位，輸入 `Login`，使得我們要產生的控制器名稱為 `LoginController`

* 點選右下方的 \[加入] 按鈕

![New Controller](Images/NewController.png)

* 將這個新產生的控制器 `LoginController` 以底下程式碼替換

  > 這裡尚未做任何使用者帳號與密碼的身分驗證程式碼

```csharp
using AuthToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthToken.Controllers
{
    public class LoginController : ApiController
    {
        [HttpGet]
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "",
                Payload = "Access Token"
            };
        }
    }
}
```

## 建立無須驗證便可存取的 Controllers

* 滑鼠右擊 \[Controllers] 資料夾，選取 \[加入] > \[控制器]

* 在 \[新增 Scaffold] 對話窗中，選擇 \[Web API2 控制器 - 空白]

* 點選右下方的 \[新增] 按鈕

* 當出現 \[加入控制器] 對話窗後，於 \[控制器名稱] 欄位，輸入 `NoAuth`，使得我們要產生的控制器名稱為 `NoAuthController`

* 點選右下方的 \[加入] 按鈕

* 將這個新產生的控制器 `NoAuthController` 以底下程式碼替換

```csharp
using AuthToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthToken.Controllers
{
    public class NoAuthController : ApiController
    {
        [HttpGet]
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "不需要提供存取權杖 Access Token 就能使用的 API",
                Payload = new string[] { "無須存取權杖1", "無須存取權杖2" }
            };
        }
    }
}
```

## 建立需要通過身分驗證便可存取的 Controllers

* 滑鼠右擊 \[Controllers] 資料夾，選取 \[加入] > \[控制器]

* 在 \[新增 Scaffold] 對話窗中，選擇 \[Web API2 控制器 - 空白]

* 點選右下方的 \[新增] 按鈕

* 當出現 \[加入控制器] 對話窗後，於 \[控制器名稱] 欄位，輸入 `NeedAuth`，使得我們要產生的控制器名稱為 `NeedAuthController`

* 點選右下方的 \[加入] 按鈕

* 將這個新產生的控制器 `NeedAuthController` 以底下程式碼替換

  > 這裡尚未進行存取權杖的驗證與取回權杖內的資料程式碼

```csharp
using AuthToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthToken.Controllers
{
    public class NeedAuthController : ApiController
    {
        [HttpGet]
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "需要提供存取權杖 Access Token 才能使用的 API",
                Payload = new string[] { "有提供存取權杖1", "有提供存取權杖2" }
            };
        }
    }
}
```

## 進行驗證專案可以正常運作

* 請建置該專案，確認沒有錯誤產生

* 執行該專案

  > 您專案的開啟網站的埠 (Port) 可能會與這個專案不同，請自行修改 Port 號碼成為您專案用的 Port 號碼

* 開啟 `PostMan` 工具，選擇 Http 方法為 `GET`

* 在 URL 位址輸入 `http://localhost:54891/api/Login`

* 確認得到底下結果

```json
{
    "Success": true,
    "Message": "",
    "Payload": "Access Token"
}
```

* 在 URL 位址輸入 `http://localhost:54891/api/NoAuth`

* 確認得到底下結果

```json
{
    "Success": true,
    "Message": "不需要提供存取權杖 Access Token 就能使用的 API",
    "Payload": [
        "無須存取權杖1",
        "無須存取權杖2"
    ]
}
```

* 在 URL 位址輸入 `http://localhost:54891/api/needAuth`

* 確認得到底下結果

```json
{
    "Success": true,
    "Message": "需要提供存取權杖 Access Token 才能使用的 API",
    "Payload": [
        "有提供存取權杖1",
        "有提供存取權杖2"
    ]
}
```

> 這個練習的完成專案，可以參考資料夾 \[02 Setup]

# 03 設計HTTP基本認證，並回傳有效存取權杖

在這裡，我們將會繼續剛剛完成的專案，設計 LoginController 可以進行 `HTTP基本認證`，相關 HTTP基本認證 說明，可以參考該[連結](https://zh.wikipedia.org/wiki/HTTP%E5%9F%BA%E6%9C%AC%E8%AE%A4%E8%AF%81)

* 在 LoginController.cs 檔案中，加入底下命名空間參考

```csharp
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
```

* 將 `LoginController` 類別，修正成為底下程式碼

```csharp
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
                // 帳號與密碼比對正確，回傳帳密比對正確
                response = this.Request.CreateResponse<APIResult>(HttpStatusCode.OK, new APIResult()
                {
                    Success = true,
                    Message = $"帳號:{Account} / 密碼:{Password}",
                    Payload = "Access Token"
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
```

## 使用 PostMan 來測試登入 API 是否可以正常運作

* 打開 PostMan 工具

* 使用這個[工具](https://www.blitter.se/utils/basic-authentication-header-generator/)，將帳號與密碼產生出基本認證的標頭；我們使用這個工具，設定帳號為 `vulcan` / 密碼為 `123abc`，得到底下標頭內容

```
Authorization: Basic dnVsY2FuOjEyM2FiYw==
```

![Basic Authentication Header Generator](Images/BasicAuthHeader.png)

* 設定 PostMand 的 Http 方法為 `GET`，增加一個 `Authorization` 標頭，其值為 `Basic dnVsY2FuOjEyM2FiYw==`，如下圖所示，並請點選右上方的 \[Send] 按鈕，將會得到底下結果：

```json
{
    "Success": true,
    "Message": "帳號:vulcan / 密碼:123abc",
    "Payload": "Access Token"
}
```

![使用帳號與密碼正確的測試](Images/AuthCorrectPostMan.png)

* 產生出不正確的帳號或密碼的標頭，底下的標頭之帳號為 `vulcan` / 密碼為 `123`，如下所示，進行測試

```
Authorization: Basic dnVsY2FuOjEyMw==
```

* 設定 PostMand 的 Http 方法為 `GET`，增加一個 `Authorization` 標頭，其值為 `Basic dnVsY2FuOjEyMw==`，如下圖所示，並請點選右上方的 \[Send] 按鈕，將會得到底下結果：

```json
{
    "Success": false,
    "Message": "",
    "Payload": "帳號或密碼不正確"
}
```

![使用帳號與密碼不正確的測試](Images/AuthIncorrectPostMan.png)

* 接著，不要設定 Authorization 標頭，進行測試，將會得到底下結果：

```json
{
    "Success": false,
    "Message": "",
    "Payload": "沒有收到帳號與密碼"
}
```

![沒有使用 Authorization 標頭的測試](Images/AuthNoAuthorizationPostMan.png)

## 產生使用者的存取權杖 Access Token

* 在 LoginController.cs 檔案中，加入底下命名空間參考

```csharp
using AuthToken.Helpers;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
```

* 找到這行註解 `// 帳號與密碼比對正確，回傳帳密比對正確`，在這行註解的上方，加入底下程式碼

```csharp
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

```

* 找到這行註解 `// 帳號與密碼比對正確，回傳帳密比對正確`，在這行註解的下方，將這行程式碼 `this.Request.CreateResponse`，修改成為底下程式碼，這樣，當使用者傳入一個合法的帳號與密碼，就可以回傳這次身分驗證的存取權杖

```csharp
response = this.Request.CreateResponse<APIResult>(HttpStatusCode.OK, new APIResult()
{
    Success = true,
    Message = $"帳號:{Account} / 密碼:{Password}",
    Payload = $"{jwtToken}"
});
```

## 使用 PostMan 來測試登入 API 是否可以回傳存取權杖 Access Token

* 打開 PostMan 工具

* 使用這個[工具](https://www.blitter.se/utils/basic-authentication-header-generator/)，將帳號與密碼產生出基本認證的標頭；我們使用這個工具，設定帳號為 `vulcan` / 密碼為 `123abc`，得到底下標頭內容

```
Authorization: Basic dnVsY2FuOjEyM2FiYw==
```

![Basic Authentication Header Generator](Images/BasicAuthHeader.png)

* 設定 PostMand 的 Http 方法為 `GET`，增加一個 `Authorization` 標頭，其值為 `Basic dnVsY2FuOjEyM2FiYw==`，如下圖所示，並請點選右上方的 \[Send] 按鈕，將會得到底下結果：

```json
{
    "Success": true,
    "Message": "帳號:vulcan / 密碼:123abc",
    "Payload": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ2dWxjYW4iLCJleHAiOjE1MTY5NTg4NzUuMCwicm9sZSI6WyJNYW5hZ2VyIiwiUGVvcGxlIl19.cHAktZhijjdbbP5mhb1ICwGdN1GwUrfAyPObq3ZMi-4"
}
```

其中，這個字串 

`eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ2dWxjYW4iLCJleHAiOjE1MTY5NTg4NzUuMCwicm9sZSI6WyJNYW5hZ2VyIiwiUGVvcGxlIl19.cHAktZhijjdbbP5mhb1ICwGdN1GwUrfAyPObq3ZMi-4`

就是存取權杖，日後要存取需要通過身分驗證的 API 的時候，只需要提供這個存取權杖，後端 Web API就會知道，這次呼叫 Web API是由哪個使用者來呼叫的。

![使用帳號與密碼正確的測試](Images/AuthTokenPostMan.png)

> 這個練習的完成專案，可以參考資料夾 \[03 BasicAuth]

# 04 設計 Web API 需要有存取權杖，才能夠存取這些API服務

首先，我們來練習，如何在用戶端的專案內，使用 `HttpClient` 抓取到存取權杖，接著使用不同情境的存取權杖，學習如何將存取權杖傳送到後端 Web API 專案上，接著，驗證與分析這個存取權證是否正確，並回傳 Web API 的執行結果。

## 如何撰寫用戶端的程式碼，取得存取權杖

* 滑鼠右擊方案 \[AuthTokenSolution]，選擇 \[加入] > \[新增專案]

* 在 \[新增專案] 對話窗左半部，點選 \[已安裝] > \[Visual C#]

* 在這個對話的中間上方，選擇 \[.NET Framework 4.6.1]

* 接著選取 [主控台應用程式 (.NET Framework)]

* 在下方 \[名稱] 欄位，輸入 `GetAccessToken`

* 最後，點選右下方的 \[確定] 按鈕

* 滑鼠右擊主控台應用程式 `GetAccessToken` 的參考節點

* 選擇 \[管理 NuGet 套件]

* 在 \[NuGet: GetAccessToken] 視窗中，點選 \[瀏覽] 標籤頁次

* 在 \[搜尋] 文字輸入盒內，輸入 `JSON.NET`，搜尋這個套件

* 取消勾選 \[包含搶鮮版] 檢查盒，選擇第一個套件 `Newtonsoft.Json`

* 點選 \[安裝] 按鈕

* 將 `GetAccessToken` 專案內的 `Program.cs` 檔案內容，使用底下程式碼替換

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GetAccessToken
{
    class Program
    {
        static void Main(string[] args)
        {
            var TARGETURL = "http://localhost:54891/api/Login";

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    var byteArray = Encoding.ASCII.GetBytes("vulcan:123abc");
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", 
                        Convert.ToBase64String(byteArray));

                    HttpResponseMessage response = client.GetAsync(TARGETURL).Result;
                    HttpContent content = response.Content;
                    Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);
                    string result =  content.ReadAsStringAsync().Result;
                    var fooAPIResult = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResult>(result);
                    Console.WriteLine($"存取權杖為 : {fooAPIResult.Payload}");
                    Console.WriteLine("Press any key for continuing...");
                    Console.ReadKey();
                }
            };

        }
    }

    /// <summary>
    /// 呼叫 API 回傳的制式格式
    /// </summary>
    public class APIResult
    {
        /// <summary>
        /// 此次呼叫 API 是否成功
        /// </summary>
        public bool Success { get; set; } = true;
        /// <summary>
        /// 呼叫 API 失敗的錯誤訊息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 呼叫此API所得到的其他內容
        /// </summary>
        public object Payload { get; set; }
    }

}
```

* 此時，您的方案內共有兩個專案，預設起始專案應該為 `AuthToken`

* 按下 \[F5] 按鍵，開始進行 Web API 專案進行除錯

* 滑鼠右擊專案 `GetAccessToken`，點選 \[偵錯] > \[開始新執行個體]

* 此時，會顯示一個命令提示字元視窗，如下圖，您會看到，我們已經把存取權杖顯示在螢幕上。

![取得存取權杖](Images/ClientGetToken.png)

* 請把這個存取權杖複製到剪貼簿內。

* 開啟 [jwt.io](https://jwt.io/) 網頁

* 在 `Encoded` 區域，將這個存取權杖貼到這裡，此時，您會看到右半部將會出現解碼後的這個存取權杖詳細內容。

![存取權杖解碼](Images/DecodeToken.png)

## 建立檢查是否存在合法的存取權杖

* 滑鼠右擊專案 \[AuthToken]，選擇 \[加入] > \[新增資料夾] 

* 在新增的資料夾位置，輸入 `Filters` 資料夾

* 滑鼠右擊 \[Filters] 資料夾，選取 \[加入] > \[類別]

* 在 \[新增項目] 對話窗中，在下方的 \[名稱] 欄位中，輸入 `JwtAuthAttribute`

* 點選 \[新增] 按鈕，完成新增這個類別檔案

* 將這個新產生的類別 `JwtAuthAttribute` 以底下程式碼替換

```csharp
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
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace AuthToken.Filters
{
    public class JwtAuthAttribute : AuthorizeAttribute
    {
        public string ErrorMessage { get; set; } = "";
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(ErrorMessage) == false)
            {
                setErrorResponse(actionContext, ErrorMessage);
            }
            else
            {
                base.HandleUnauthorizedRequest(actionContext);
            }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // TODO: key應該移至config
            if (actionContext.Request.Headers.Authorization == null || actionContext.Request.Headers.Authorization.Scheme != "Bearer")
            {
                setErrorResponse(actionContext, "沒有看到存取權杖錯誤");
            }
            else
            {
                try
                {
                    #region 進行存取權杖的解碼
                    string secretKey = MainHelper.SecretKey;
                    var json = new JwtBuilder()
                        .WithAlgorithm(new HMACSHA256Algorithm())
                        .WithSecret(secretKey)
                        .MustVerifySignature()
                        .Decode<Dictionary<string, object>>(actionContext.Request.Headers.Authorization.Parameter);
                    #endregion

                    #region 將存取權杖所夾帶的內容取出來
                    var fooRole = json["role"] as Newtonsoft.Json.Linq.JArray;
                    var fooRoleList = fooRole.Select(x => (string)x).ToList<string>();
                    #endregion

                    #region 將存取權杖的夾帶欄位，儲存到 HTTP 要求的屬性
                    actionContext.Request.Properties.Add("user", json["iss"] as string);
                    actionContext.Request.Properties.Add("role", fooRoleList);
                    #endregion

                    #region 設定目前 HTTP 要求的安全性資訊
                    var fooPrincipal =
                        new GenericPrincipal(new GenericIdentity(json["iss"] as string, "MyPassport"), fooRoleList.ToArray());
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = fooPrincipal;
                    }
                    #endregion

                    #region 角色權限檢查(檢查控制器或動作之屬性(Attribute上設的 Roles的設定內容)
                    if (string.IsNullOrEmpty(Roles) == false)
                    {
                        // 是否有找到匹配的角色設定
                        bool fooCheckRoleResult = false;
                        // 切割成為多個角色成員
                        var fooConditionRoles = Roles.Split(',');
                        // 逐一檢查，這個使用用者是否有在這個角色條件中
                        foreach (var item in fooConditionRoles)
                        {
                            var fooInRole = fooPrincipal.IsInRole(item.Trim());
                            if (fooInRole == true)
                            {
                                fooCheckRoleResult = true;
                                break;
                            }
                        }

                        if(fooCheckRoleResult == false)
                        {
                            setErrorResponse(actionContext, "無效的角色設定，沒有權限使用這個 API");
                        }
                    }
                    #endregion

                }
                catch (TokenExpiredException)
                {
                    setErrorResponse(actionContext, "權杖已經逾期");
                }
                catch (SignatureVerificationException)
                {
                    setErrorResponse(actionContext, "權杖似乎不正確，沒有正確的數位簽名");
                }
                catch (Exception ex)
                {
                    setErrorResponse(actionContext, $"權杖解析發生異常 : {ex.Message}");
                }
            }

            base.OnAuthorization(actionContext);
        }

        private void setErrorResponse(HttpActionContext actionContext, string message)
        {
            ErrorMessage = message;
            var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
            response.Content = new ObjectContent<APIResult>(new APIResult()
            {
                Success = false,
                Message = ErrorMessage,
                Payload = null
            }, new JsonMediaTypeFormatter());
            actionContext.Response = response;
        }
    }
}
```

* 現在，只要把這個 `JwtAuthAttribute` 屬性，套用到相關的控制器 (Controller) 或者動作 (Action) 方法上，因此，當要存取這些控制器或者動作的時候，就必須要提供有效存取權杖，方能呼叫這些 Web API，否則，會有錯誤回報。

  現在，我們打開這個 `NeedAuthController.cs` 檔案，將類別 `NeedAuthController` 替換成為底下程式碼。

```csharp
[JwtAuth]
public class NeedAuthController : ApiController
{
    [HttpGet]
    public APIResult Get()
    {
        var fooUser = Request.Properties["user"] as string;
        
        return new APIResult()
        {
            Success = true,
            Message = $"授權使用者為 {fooUser}",
            Payload = new string[] { "有提供存取權杖1", "有提供存取權杖2" }
        };
    }
}
```

* 現在，執行這個 Web API 專案，並且使用 PostMan 來存取這個網址 [http://localhost:54891/api/needAuth](http://localhost:54891/api/needAuth)，但是，看到底下的回傳內容，這表示，當要存取這個 `NeedAuth` Web API 的時候，我們需要提供有效的存取權杖。

```json
{
    "Success": false,
    "Message": "沒有看到存取權杖錯誤",
    "Payload": null
}
```

* 讓我們重新執行這個網址 [http://localhost:54891/api/Login](http://localhost:54891/api/Login)，得到最新的存取權杖

```json
{
    "Success": true,
    "Message": "帳號:vulcan / 密碼:123abc",
    "Payload": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ2dWxjYW4iLCJleHAiOjE1MTY5Nzk5MzUuMCwicm9sZSI6WyJNYW5hZ2VyIiwiUGVvcGxlIl19.4iGEoBA0SoCiSTBXdAn0J3J_hvVNS1ZeZIP6OeJAPm8"
}
```

* 我們在 PostMan 上，找到這個 API [http://localhost:54891/api/needAuth](http://localhost:54891/api/needAuth) ，在這個API頁面，加入 `Authorization` 標頭，該標頭的值要為 `Bearer Token`，在我們這個例子中就是如下的標頭定義；當設定完成之後，按下 \[Send] 按鈕，就發現到我們現在可以呼叫這個 Web API了。

```
Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ2dWxjYW4iLCJleHAiOjE1MTY5Nzk5MzUuMCwicm9sZSI6WyJNYW5hZ2VyIiwiUGVvcGxlIl19.4iGEoBA0SoCiSTBXdAn0J3J_hvVNS1ZeZIP6OeJAPm8
```

![](Images/APIWithToken.png)

* 但是，若我們竄改了這個存取權杖，或者權杖的內容被刪去了幾個字元，在這裡，我們把剛剛的權杖刪除最後兩個字元，因此，底下的標題將是我們要送出的內容

```
Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ2dWxjYW4iLCJleHAiOjE1MTY5Nzk5MzUuMCwicm9sZSI6WyJNYW5hZ2VyIiwiUGVvcGxlIl19.4iGEoBA0SoCiSTBXdAn0J3J_hvVNS1ZeZIP6OeJAP
```

* 此時，您會得到底下的結果，告訴您的權杖是有問題，並且無法使用這個 Web API

```json
{
    "Success": false,
    "Message": "權杖解析發生異常 : Illegal base64url string.",
    "Payload": null
}
```

* 由於，發出的存取權杖有效期限為一個小時，若你使用一個小時前發出的權杖，來呼叫這個 Web API，例如，使用底下的標頭。

```
Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ2dWxjYW4iLCJleHAiOjE1MTY5Njk0MTUuMCwicm9sZSI6WyJNYW5hZ2VyIiwiUGVvcGxlIl19.8KgvjlRONFIpSnIYD2iTjpY4IWH9vn_ntvI_LwhvBHQ
```

* 這個時候，就會得到底下的 `權杖已經逾期` 的訊息。

```json
{
    "Success": false,
    "Message": "權杖已經逾期",
    "Payload": null
}
```

> 這個練習的完成專案，可以參考資料夾 \[04 CallWithToken]





