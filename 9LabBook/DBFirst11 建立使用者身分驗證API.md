# DBFirst11 建立使用者身分驗證API

在這個練習中，我們將會設計一個使用者登入驗證的 Web API，這個 Web API 將會提供兩種身分驗證的管理，分別是 `HTTP 基本認證` 與 `POST 身分驗證`。

## 建立使用者身分驗證控制器

* 滑鼠右擊 `Controllers` 資料夾，選擇 `加入` > `控制器`

  ![](Images/DBFirst25.png)

* 在 `新增 Scaffold` 對話窗中，點選 `Web API 2 控制器 - 空白` > `新增`

* 在 `加入控制器` 對話窗中，輸入 `Login`，如同底下畫面，最後點選 `加入` 按鈕

  ![](Images/AddController5.png)

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using LOBFormDBFirst.Helpers;
using LOBFormDBFirst.Models;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
```

* 將新增的類別以底下程式碼替換

![](Icons/csharp.png)

```csharp
public class LoginController : ApiController
{
    private LOBFormEntities db = new LOBFormEntities();
    private APIResult fooResult = new APIResult();
    public string Account { get; set; }
    public string Password { get; set; }
 
    public async Task<APIResult> Get()
    {
        if (CanHandleAuthentication(this.Request) == true)
        {
            #region 檢查帳號與密碼是否正確
            // 這裡可以修改成為與後端資料庫內的使用者資料表進行比對
            var expectAccount = Account;
            var expectPassword = Password;
            var fooItem = await db.LOBMyUsers.FirstOrDefaultAsync(x => x.EmployeeID == expectAccount && x.Password == expectPassword);
            if (fooItem != null)
            {
                #region 產生這次通過身分驗證的存取權杖 Access Token
                string secretKey = MainHelper.SecretKey;
                #region 設定該存取權杖的有效期限
                IDateTimeProvider provider = new UtcDateTimeProvider();
                // 這個 Access Token只有一個小時有效
                var now = provider.GetNow().AddHours(24 * 7);
                var unixEpoch = UnixEpoch.Value; // 1970-01-01 00:00:00 UTC
                var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);
                #endregion
 
                string[] fooRole;
                if (fooItem.IsManager == true)
                {
                    fooRole = new string[] { "Manager" };
                }
                else
                {
                    fooRole = new string[0];
                }
                var jwtToken = new JwtBuilder()
                      .WithAlgorithm(new HMACSHA256Algorithm())
                      .WithSecret(secretKey)
                      .AddClaim("iss", Account)
                      .AddClaim("exp", secondsSinceEpoch)
                      .AddClaim("role", fooRole)
                      .AddClaim("manager", fooItem.IsManager)
                      .Build();
                #endregion
 
                // 帳號與密碼比對正確，回傳帳密比對正確
                this.Request.CreateResponse(HttpStatusCode.OK);
                fooResult = new APIResult()
                {
                    Success = true,
                    Message = $"",
                    TokenFail = false,
                    Payload = new UserLoginResultModel()
                    {
                        AccessToken = $"{jwtToken}",
                        MyUser = fooItem.ToMyUsers(),
                    }
                };
            }
            else
            {
                fooResult.Success = false;
                fooResult.Message = $"使用者不存在或者帳號、密碼不正確";
                fooResult.TokenFail = false;
                fooResult.Payload = null;
            }
            #endregion
        }
        else
        {
            // 沒有收到正確格式的 Authorization 內容，回傳無法驗證訊息
            this.Request.CreateResponse(HttpStatusCode.Unauthorized);
            fooResult = new APIResult()
            {
                Success = false,
                TokenFail = false,
                Message = $"",
                Payload = "沒有收到帳號與密碼"
            };
        }
        return fooResult;
    }
 
    //POST: api/Login
    public async Task<APIResult> Post([FromBody] UserLoginModel UserLoginModel)
    {
        var account = UserLoginModel.Account;
        var password = UserLoginModel.Password;
        var fooItem = await db.LOBMyUsers.FirstOrDefaultAsync(x => x.EmployeeID == account && x.Password == password);
        if (fooItem != null)
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
 
            string[] fooRole;
            if (fooItem.IsManager == true)
            {
                fooRole = new string[] { "Manager" };
            }
            else
            {
                fooRole = new string[0];
            }
            var jwtToken = new JwtBuilder()
                  .WithAlgorithm(new HMACSHA256Algorithm())
                  .WithSecret(secretKey)
                  .AddClaim("iss", UserLoginModel.Account)
                  .AddClaim("exp", secondsSinceEpoch)
                  .AddClaim("role", fooRole)
                  .AddClaim("manager", fooItem.IsManager)
                  .Build();
            #endregion
 
            // 帳號與密碼比對正確，回傳帳密比對正確
            this.Request.CreateResponse(HttpStatusCode.OK);
            fooResult = new APIResult()
            {
                Success = true,
                Message = $"",
                TokenFail = false,
                Payload = new UserLoginResultModel()
                {
                    AccessToken = $"{jwtToken}",
                    MyUser = fooItem.ToMyUsers(),
                }
            };
        }
        else
        {
            fooResult.Success = false;
            fooResult.Message = $"使用者不存在或者帳號、密碼不正確";
            fooResult.TokenFail = false;
            fooResult.Payload = null;
        }
        return fooResult;
    }
 
    /// <summary>
    /// 檢查與解析 Authorization 標頭是否存在與解析用戶端傳送過來的帳號與密碼
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private bool CanHandleAuthentication(HttpRequestMessage request)
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
 
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

# 測試此控制器

* 執行此專案

## 測試 Http 基本認證的 GET 方法

* 打開 PostMan 工具

  * 選擇 Http 方法為 `Get`

  * 輸入 URL 為 `http://localhost:50490/api/Login`

    > 若您自己建立的 Web API 專案，請在這裡輸入您專案的 Port 編號

  * 點選 `Headers` 標籤頁次

  * 輸入這組 Http 標頭
  
    \[Key] 名稱為 `Authorization`
    
    \[Value] 值為 `Basic dXNlcjE6cHdkMQ==`

  * 點選 `Send` 按鈕

![PostMan](Images/DPostMan5.png)

* 請確認輸出內容是否與底下內容類似

![](Icons/Json.png)

```json
{
    "success": true,
    "tokenFail": false,
    "message": "",
    "payload": {
        "accessToken": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ1c2VyMSIsImV4cCI6MTUxNzcyMDQ0MS4wLCJyb2xlIjpbXSwibWFuYWdlciI6ZmFsc2V9._WMHyciL7Mt7ZiPMV4Qz2R3pEIJkpJqI_LXu42Xs5EI",
        "myUser": {
            "myUserId": 2,
            "departmentName": "部門0",
            "name": "使用者1",
            "employeeID": "user1",
            "password": "pwd1",
            "managerId": 1,
            "isManager": false,
            "createdAt": null,
            "updatedAt": null
        }
    }
}
```

## 測試 使用 POST 方法來進行使用者身分驗證

* 打開 PostMan 工具

  * 選擇 Http 方法為 `Post`

  * 輸入 URL 為 `http://localhost:50490/api/Login`

    > 若您自己建立的 Web API 專案，請在這裡輸入您專案的 Port 編號

  * 點選 `Headers` 標籤頁次

  * 點選 `Body` 標籤頁次

  * 選擇使用 `raw` / `JSON(application/json)`，並且輸入底下 JSON內容

![](Icons/Json.png)

```json
{
    "account": "user1",
    "password": "pwd1"
}
```

  * 點選 `Send` 按鈕

![PostMan](Images/DPostMan6.png)

* 若輸出底下內容，則表示 `使用者身分驗證` 控制器，已經成功建立完成了

![](Icons/Json.png)

```json
{
    "success": true,
    "tokenFail": false,
    "message": "",
    "payload": {
        "accessToken": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ1c2VyMSIsImV4cCI6MTUxNzcyMDUwMy4wLCJyb2xlIjpbXSwibWFuYWdlciI6ZmFsc2V9.d1Iw6aP1OYePbJCzih8AAiwL6h624x5xkbZ_-SjFjCo",
        "myUser": {
            "myUserId": 2,
            "departmentName": "部門0",
            "name": "使用者1",
            "employeeID": "user1",
            "password": "pwd1",
            "managerId": 1,
            "isManager": false,
            "createdAt": null,
            "updatedAt": null
        }
    }
}
```

# 問題研究

![](Icons/fa-question-circle30.png) 如何取得 HTTP 基本認證所傳送過來的 HTTP 標頭？

![](Icons/fa-question-circle30.png) 在這個練習中，不論是 GET 或者 POST 方法，我該如何確保帳號與密碼不會被透過網路封包監聽，簡單的取得使用者的帳號與密碼？

![](Icons/fa-question-circle30.png) 若設計使用 HTTP 基本認證 方式，帳號與密碼是否可以另外做加密保護的方式呢？

![](Icons/fa-question-circle30.png) 請試著了解如何產生存取權杖的過程？

![](Icons/fa-question-circle30.png) 如何決定存取權杖的有效期限要設定多久了？較長與較短有效期限有何差異呢？

![](Icons/fa-question-circle30.png) 如何在存取權杖中，放置使用者身分或者相關資料呢？以及，請思考為什麼要這樣做？

![](Icons/fa-question-circle30.png) 產生出來的存取權杖，我可以將這個存取權杖解碼出來，查看詳細內容嗎？

![](Icons/fa-question-circle30.png) 甚麼樣的資料要放到存取權杖內呢？

![](Icons/fa-question-circle30.png) 我想要在 Web API 內，回傳一個 APIResult 物件，但是，想要指定不同的 HTTP 狀態碼，我該如何做呢？

![](Icons/fa-question-circle30.png) 當使用者的帳號與密碼驗證失敗後，我該回傳 HttpStatusCode.OK 或者 HttpStatusCode.Unauthorized 呢？

![](Icons/fa-question-circle30.png) 在 POST 方法中，使用了 `[FromBody]` 屬性，這個屬性是做甚麼用途的呢？

![](Icons/fa-question-circle30.png) 
