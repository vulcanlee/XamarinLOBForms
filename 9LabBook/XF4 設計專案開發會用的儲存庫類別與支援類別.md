# XF4 設計專案開發會用的儲存庫類別與支援類別

![](Icons/fa-search.png) 這份文件的完成結果專案原始碼，可以參考 `XamarinLOBForms\2XFLab\04Repository`

現在，讓我們來設計呼叫 Web API 的相關支援類別，我們就可以在 Xamarin.Forms 專案內，直接使用這些設計好的 Repository 類別所提供的方法，便可以存取遠端 Web API 的服務了；當然，對於具有 CRUD 應用的 Web API，在相對應的 Repository 類別中，也會實作出相對應 CRUD 呼叫的程式碼。

# 建立專案會用到的支援類別

*使用滑鼠右鍵點選`LOBForm` .NET Standard 共用類別庫專案，選擇 \[加入] > \[新增資料夾]

* 輸入 `Helpers`

## 建立 MainHelper.cs

* 滑鼠右擊 `Helpers` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `MainHelper`，之後點選 `新增` 按鈕

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
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
    #endregion
 
    #region Repository (此處為方便開發，所以，所有的 Repository 皆為全域靜態可存取)
    #endregion
 
}
```

## 建立 StorageUtility.cs

* 滑鼠右擊 `Helpers` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `StorageUtility`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using PCLStorage;
using System.Threading.Tasks;
using System.Linq;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
/// <summary>
/// Storage 相關的 API
/// http://www.nudoq.org/#!/Packages/PCLStorage/PCLStorage/FileSystem
/// </summary>
public class StorageUtility
{
    /// <summary>
    /// 將所指定的字串寫入到指定目錄的檔案內
    /// </summary>
    /// <param name="folderName">目錄名稱</param>
    /// <param name="filename">檔案名稱</param>
    /// <param name="content">所要寫入的文字內容</param> 
    /// <returns></returns>
    public static async Task WriteToDataFileAsync(string folderPath, string folderName, string filename, string content)
    {
        // 宣告根目錄的物件變數
        IFolder rootFolder = FileSystem.Current.LocalStorage;
 
        #region 檢查傳入的參數內容是否有問題
        if (string.IsNullOrEmpty(folderName))
        {
            throw new ArgumentNullException("folderName");
        }
 
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException("filename");
        }
 
        if (string.IsNullOrEmpty(content))
        {
            throw new ArgumentNullException("content");
        }
        #endregion
 
        // 在 C# 內，要進行資源(網路、檔案、作業系統資源等)存取的時候，請要將這些程式碼 try ... catch
        try
        {
            #region 建立與取得指定路徑內的資料夾
            string[] fooPaths = folderName.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            IFolder folder = rootFolder;
            foreach (var item in fooPaths)
            {
                folder = await folder.CreateFolderAsync(item, CreationCollisionOption.OpenIfExists);
            }
            #endregion
 
            IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
 
            await file.WriteAllTextAsync(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
        }
    }
 
    /// <summary>
    /// 從指定目錄的檔案內將文字內容讀出
    /// </summary>
    /// <param name="folderName">目錄名稱</param>
    /// <param name="filename">檔案名稱</param>
    /// <returns>文字內容</returns>
    public static async Task<string> ReadFromDataFileAsync(string folderPath, string folderName, string filename)
    {
        string content = "";
 
        // 宣告根目錄的物件變數
        IFolder rootFolder = FileSystem.Current.LocalStorage;
 
        #region 檢查傳入的參數內容是否有問題
        if (string.IsNullOrEmpty(folderName))
        {
            throw new ArgumentNullException("folderName");
        }
 
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException("filename");
        }
        #endregion
 
        // 在 C# 內，要進行資源(網路、檔案、作業系統資源等)存取的時候，請要將這些程式碼 try ... catch
        try
        {
            #region 建立與取得指定路徑內的資料夾
            string[] fooPaths = folderName.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            IFolder folder = rootFolder;
            foreach (var item in fooPaths)
            {
                folder = await folder.CreateFolderAsync(item, CreationCollisionOption.OpenIfExists);
            }
            #endregion
 
            var fooallf = await folder.GetFilesAsync();
            var fooExist = fooallf.FirstOrDefault(x => x.Name == filename);
            if (fooExist != null)
            {
                IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
 
                content = await file.ReadAllTextAsync();
            }
            else
            {
                content = "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
        }
 
        return content.Trim();
    }
}
```

# 建立專案會用到的儲存庫類別

*使用滑鼠右鍵點選`LOBForm` .NET Standard 共用類別庫專案，選擇 \[加入] > \[新增資料夾]

* 輸入 `Repositories`

## 建立 SystemStatusRepository.cs

* 滑鼠右擊 `Repositories` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `SystemStatusRepository`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
namespace LOBForm.Repositories
{
    public class SystemStatusRepository
    {
        public SystemStatus Item { get; set; }
        /// <summary>
        /// 將資料寫入到檔案內
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            string data = JsonConvert.SerializeObject(Item);
            await StorageUtility.WriteToDataFileAsync("", MainHelper.資料主目錄, MainHelper.SystemStatusFileName, data);
        }
 
        /// <summary>
        /// 將資料讀取出來
        /// </summary>
        /// <returns></returns>
        public async Task<SystemStatus> ReadAsync()
        {
            string data = "";
            data = await StorageUtility.ReadFromDataFileAsync("", MainHelper.資料主目錄, MainHelper.SystemStatusFileName);
            Item = JsonConvert.DeserializeObject<SystemStatus>(data);
            if (Item == null)
            {
                Item = new SystemStatus();
            }
            return Item;
        }
    }
}
```

## 建立 ProjectRepository.cs

* 滑鼠右擊 `Repositories` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `ProjectRepository`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
public class ProjectRepository
{
    /// <summary>
    /// 呼叫 API 回傳後，回報的結果內容
    /// </summary>
    public APIResult fooAPIResult { get; set; } = new APIResult();
    /// <summary>
    /// 身分驗證成功後的使用者紀錄
    /// </summary>
    public List<Project> Items { get; set; } = new List<Project>();
 
 
    /// <summary>
    /// 使用者身分驗證：登入
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<APIResult> GetAsync()
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.ProjectAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if(response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                // 將 Payload 裡面的內容，反序列化為真實 .NET 要用到的資料
                                Items = JsonConvert.DeserializeObject<List<Project>>(fooAPIResult.Payload.ToString());
                                if (Items == null)
                                {
                                    Items = new List<Project>();
 
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = $"回傳的 API 內容不正確 : {fooAPIResult.Payload.ToString()}",
                                        Payload = null,
                                    };
                                }
                                else
                                {
                                    await SaveAsync();
                                }
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<Project>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            await SaveAsync();
                            #endregion
                        }
                        else
                        {
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"應用程式呼叫 API 發生異常{Environment.NewLine}錯誤代碼:{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                TokenFail = false,
                                Payload = null,
                            };
                        }
                    }
                    else
                    {
                        #region API 的狀態碼為 不成功
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                            Payload = null,
                        };
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        TokenFail = false,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    /// <summary>
    /// 將資料寫入到檔案內
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        string data = JsonConvert.SerializeObject(Items);
        await StorageUtility.WriteToDataFileAsync("", MainHelper.資料主目錄, MainHelper.ProjectAPIName, data);
    }
 
    /// <summary>
    /// 將資料讀取出來
    /// </summary>
    /// <returns></returns>
    public async Task<List<Project>> ReadAsync()
    {
        string data = "";
        data = await StorageUtility.ReadFromDataFileAsync("", MainHelper.資料主目錄, MainHelper.ProjectAPIName);
        Items = JsonConvert.DeserializeObject<List<Project>>(data);
        if (Items == null)
        {
            Items = new List<Project>();
        }
        return Items;
    }
}
```

## 建立 OnCallPhoneRepository.cs

* 滑鼠右擊 `OnCallPhoneRepository` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `APIResult`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
public class OnCallPhoneRepository
{
    /// <summary>
    /// 呼叫 API 回傳後，回報的結果內容
    /// </summary>
    public APIResult fooAPIResult { get; set; } = new APIResult();
    /// <summary>
    /// 身分驗證成功後的使用者紀錄
    /// </summary>
    public List<OnCallPhone> Items { get; set; } = new List<OnCallPhone>();
 
 
    /// <summary>
    /// 使用者身分驗證：登入
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<APIResult> GetAsync()
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.OnCallPhoneAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if(response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                // 將 Payload 裡面的內容，反序列化為真實 .NET 要用到的資料
                                Items = JsonConvert.DeserializeObject<List<OnCallPhone>>(fooAPIResult.Payload.ToString());
                                if (Items == null)
                                {
                                    Items = new List<OnCallPhone>();
 
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = $"回傳的 API 內容不正確 : {fooAPIResult.Payload.ToString()}",
                                        Payload = null,
                                    };
                                }
                                else
                                {
                                    await SaveAsync();
                                }
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<OnCallPhone>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"應用程式呼叫 API 發生異常{Environment.NewLine}錯誤代碼:{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                TokenFail = false,
                                Payload = null,
                            };
                        }
                    }
                    else
                    {
                        #region API 的狀態碼為 不成功
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                            Payload = null,
                        };
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    /// <summary>
    /// 將資料寫入到檔案內
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        string data = JsonConvert.SerializeObject(Items);
        await StorageUtility.WriteToDataFileAsync("", MainHelper.資料主目錄, MainHelper.OnCallPhoneAPIName, data);
    }
 
    /// <summary>
    /// 將資料讀取出來
    /// </summary>
    /// <returns></returns>
    public async Task<List<OnCallPhone>> ReadAsync()
    {
        string data = "";
        data = await StorageUtility.ReadFromDataFileAsync("", MainHelper.資料主目錄, MainHelper.OnCallPhoneAPIName);
        Items = JsonConvert.DeserializeObject<List<OnCallPhone>>(data);
        if (Items == null)
        {
            Items = new List<OnCallPhone>();
        }
        return Items;
    }
}
```

## 建立 LeaveCategoryRepository.cs

* 滑鼠右擊 `Repositories` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `LeaveCategoryRepository`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
public class LeaveCategoryRepository
{
    /// <summary>
    /// 呼叫 API 回傳後，回報的結果內容
    /// </summary>
    public APIResult fooAPIResult { get; set; } = new APIResult();
    /// <summary>
    /// 身分驗證成功後的使用者紀錄
    /// </summary>
    public List<LeaveCategory> Items { get; set; } = new List<LeaveCategory>();
 
 
    /// <summary>
    /// 使用者身分驗證：登入
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<APIResult> GetAsync()
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.LeaveCategoryAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if(response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                // 將 Payload 裡面的內容，反序列化為真實 .NET 要用到的資料
                                Items = JsonConvert.DeserializeObject<List<LeaveCategory>>(fooAPIResult.Payload.ToString());
                                if (Items == null)
                                {
                                    Items = new List<LeaveCategory>();
 
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = $"回傳的 API 內容不正確 : {fooAPIResult.Payload.ToString()}",
                                        Payload = null,
                                    };
                                }
                                else
                                {
                                    await SaveAsync();
                                }
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<LeaveCategory>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            await SaveAsync();
                            #endregion
                        }
                        else
                        {
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"應用程式呼叫 API 發生異常{Environment.NewLine}錯誤代碼:{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                TokenFail = false,
                                Payload = null,
                            };
                        }
                    }
                    else
                    {
                        #region API 的狀態碼為 不成功
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                            Payload = null,
                        };
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    /// <summary>
    /// 將資料寫入到檔案內
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        string data = JsonConvert.SerializeObject(Items);
        await StorageUtility.WriteToDataFileAsync("", MainHelper.資料主目錄, MainHelper.LeaveCategoryAPIName, data);
    }
 
    /// <summary>
    /// 將資料讀取出來
    /// </summary>
    /// <returns></returns>
    public async Task<List<LeaveCategory>> ReadAsync()
    {
        string data = "";
        data = await StorageUtility.ReadFromDataFileAsync("", MainHelper.資料主目錄, MainHelper.LeaveCategoryAPIName);
        Items = JsonConvert.DeserializeObject<List<LeaveCategory>>(data);
        if (Items == null)
        {
            Items = new List<LeaveCategory>();
        }
        return Items;
    }
}
```

## 建立 LoginRepository.cs

* 滑鼠右擊 `Repositories` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `LoginRepository`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
public class LoginRepository
{
    /// <summary>
    /// 呼叫 API 回傳後，回報的結果內容
    /// </summary>
    public APIResult fooAPIResult { get; set; } = new APIResult();
    /// <summary>
    /// 身分驗證成功後的使用者紀錄
    /// </summary>
    public UserLoginResultModel Item { get; set; } = new UserLoginResultModel();
 
 
    /// <summary>
    /// 使用者身分驗證：登入 (使用 GET)
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<APIResult> GetAsync(string account, string password)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooUrl = $"{MainHelper.UserLoginAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 將帳號與密碼進行編碼
                    var byteArray = Encoding.ASCII.GetBytes($"{account}:{password}");
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(byteArray));
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooUrl}";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                Item = JsonConvert.DeserializeObject<UserLoginResultModel>
                                    (fooAPIResult.Payload.ToString(), new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
 
                                var fooSystemStatusRepository = new SystemStatusRepository();
                                await fooSystemStatusRepository.ReadAsync();
                                fooSystemStatusRepository.Item.AccessToken = Item.AccessToken;
                                await fooSystemStatusRepository.SaveAsync();
 
                                await SaveAsync();
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Item = new UserLoginResultModel();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Item,
                                };
                                #endregion
                            }
                            await SaveAsync();
                            #endregion
                        }
                        else
                        {
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"應用程式呼叫 API 發生異常{Environment.NewLine}錯誤代碼:{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                TokenFail = false,
                                Payload = null,
                            };
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    /// <summary>
    /// 使用者身分驗證：登入 (使用 POST)
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<APIResult> PostAsync(string account, string password)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooUrl = $"{MainHelper.UserLoginAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 使用 FormUrlEncodedContent 產生要 Post 的資料
 
                    var fooUserLoginModel = new UserLoginModel()
                    {
                        Account = account,
                        Password = password
                    };
 
                    // 強型別用法
                    // https://docs.microsoft.com/zh-tw/dotnet/csharp/language-reference/keywords/nameof
                    Dictionary<string, string> formDataDictionary = new Dictionary<string, string>()
                    {
                        { nameof(fooUserLoginModel.Account), fooUserLoginModel.Account },
                        { nameof(fooUserLoginModel.Password), fooUserLoginModel.Password },
                    };
 
                    // https://msdn.microsoft.com/zh-tw/library/system.net.http.formurlencodedcontent(v=vs.110).aspx
                    var formData = new FormUrlEncodedContent(formDataDictionary);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooUrl}";
                    #endregion
 
                    response = await client.PostAsync(fooFullUrl, formData);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                Item = JsonConvert.DeserializeObject<UserLoginResultModel>
                                    (fooAPIResult.Payload.ToString(), new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
 
                                var fooSystemStatusRepository = new SystemStatusRepository();
                                await fooSystemStatusRepository.ReadAsync();
                                fooSystemStatusRepository.Item.AccessToken = Item.AccessToken;
                                await fooSystemStatusRepository.SaveAsync();
 
                                await SaveAsync();
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Item = new UserLoginResultModel();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Item,
                                };
                                #endregion
                            }
                            await SaveAsync();
                            #endregion
                        }
                    }
                    else
                    {
                        #region API 的狀態碼為 不成功
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                            Payload = null,
                        };
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    /// <summary>
    /// 將資料寫入到檔案內
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        string data = JsonConvert.SerializeObject(Item);
        await StorageUtility.WriteToDataFileAsync("", MainHelper.資料主目錄, MainHelper.UserLoginAPIName, data);
    }
 
    /// <summary>
    /// 將資料讀取出來
    /// </summary>
    /// <returns></returns>
    public async Task<UserLoginResultModel> ReadAsync()
    {
        string data = "";
        data = await StorageUtility.ReadFromDataFileAsync("", MainHelper.資料主目錄, MainHelper.UserLoginAPIName);
        Item = JsonConvert.DeserializeObject<UserLoginResultModel>(data);
        if (Item == null)
        {
            Item = new UserLoginResultModel();
        }
        return Item;
    }
}
```

## 建立 WorkingLogRepository.cs

* 滑鼠右擊 `Repositories` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `WorkingLogRepository`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
public class WorkingLogRepository
{
    /// <summary>
    /// 呼叫 API 回傳後，回報的結果內容
    /// </summary>
    public APIResult fooAPIResult { get; set; } = new APIResult();
    /// <summary>
    /// 工作日誌紀錄
    /// </summary>
    public List<WorkingLog> Items { get; set; } = new List<WorkingLog>();
 
 
    /// <summary>
    /// 取得使用者的工作日誌紀錄
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<APIResult> GetByUserIDAsync(string employeeID)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.WorkingLogByUserIDAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    // http://xamarinlobform.azurewebsites.net/api/WorkingLog/ByUserID?userID=user1&mode=user
                    var fooFullUrl = $"{FooAPIUrl}?userID={employeeID}&Mode=user";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                // 將 Payload 裡面的內容，反序列化為真實 .NET 要用到的資料
                                Items = JsonConvert.DeserializeObject<List<WorkingLog>>(fooAPIResult.Payload.ToString());
                                if (Items == null)
                                {
                                    Items = new List<WorkingLog>();
 
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = $"回傳的 API 內容不正確 : {fooAPIResult.Payload.ToString()}",
                                        Payload = null,
                                    };
                                }
                                else
                                {
                                    await SaveAsync();
                                }
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<WorkingLog>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                    TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            Items = new List<WorkingLog>();
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            TokenFail = false,
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> GetAsync()
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.WorkingLogAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                // 將 Payload 裡面的內容，反序列化為真實 .NET 要用到的資料
                                Items = JsonConvert.DeserializeObject<List<WorkingLog>>(fooAPIResult.Payload.ToString());
                                if (Items == null)
                                {
                                    Items = new List<WorkingLog>();
 
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = $"回傳的 API 內容不正確 : {fooAPIResult.Payload.ToString()}",
                                        Payload = null,
                                    };
                                }
                                else
                                {
                                    await SaveAsync();
                                }
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<WorkingLog>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            Items = new List<WorkingLog>();
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> GetAsync(int id)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.WorkingLogAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}/{id}";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<WorkingLog>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            Items = new List<WorkingLog>();
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> PostAsync(WorkingLog workingLog)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.WorkingLogAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    var fooJSON = JsonConvert.SerializeObject(workingLog);
                    // https://msdn.microsoft.com/zh-tw/library/system.net.http.stringcontent(v=vs.110).aspx
                    using (var fooContent = new StringContent(fooJSON, Encoding.UTF8, "application/json"))
                    {
                        response = await client.PostAsync(fooFullUrl, fooContent);
                    }
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<WorkingLog>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            Items = new List<WorkingLog>();
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> PutAsync(WorkingLog workingLog)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.WorkingLogAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    var fooJSON = JsonConvert.SerializeObject(workingLog);
                    // https://msdn.microsoft.com/zh-tw/library/system.net.http.stringcontent(v=vs.110).aspx
                    using (var fooContent = new StringContent(fooJSON, Encoding.UTF8, "application/json"))
                    {
                        response = await client.PutAsync(fooFullUrl, fooContent);
                    }
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<WorkingLog>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            Items = new List<WorkingLog>();
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> DeleteAsync(WorkingLog workingLog)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.WorkingLogAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                    
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}/{workingLog.WorkingLogId}";
                    #endregion
 
                    response = await client.DeleteAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<WorkingLog>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            Items = new List<WorkingLog>();
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    /// <summary>
    /// 將資料寫入到檔案內
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        string data = JsonConvert.SerializeObject(Items);
        await StorageUtility.WriteToDataFileAsync("", MainHelper.資料主目錄, MainHelper.WorkingLogAPIName, data);
    }
 
    /// <summary>
    /// 將資料讀取出來
    /// </summary>
    /// <returns></returns>
    public async Task<List<WorkingLog>> ReadAsync()
    {
        string data = "";
        data = await StorageUtility.ReadFromDataFileAsync("", MainHelper.資料主目錄, MainHelper.WorkingLogAPIName);
        Items = JsonConvert.DeserializeObject<List<WorkingLog>>(data);
        if (Items == null)
        {
            Items = new List<WorkingLog>();
        }
        return Items;
    }
}
```

## 建立 LeaveAppFormRepository.cs

* 滑鼠右擊 `Repositories` 資料夾，選擇 \[加入] > \[類別]

* 在\[名稱] 欄位內，輸入 `LeaveAppFormRepository`，之後點選 `新增` 按鈕

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
```

* 使用底下程式碼，替換剛剛產生的類別

![](Icons/csharp.png)

```csharp
public class LeaveAppFormRepository
{
    /// <summary>
    /// 呼叫 API 回傳後，回報的結果內容
    /// </summary>
    public APIResult fooAPIResult { get; set; } = new APIResult();
    /// <summary>
    /// 請假單紀錄
    /// </summary>
    public List<LeaveAppForm> Items { get; set; } = new List<LeaveAppForm>();
 
 
    /// <summary>
    /// 取得使用者的請假單紀錄
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<APIResult> PostByUserIDAsync(LeaveAppFormByUserModel leaveAppFormByUserModel)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.LeaveAppFormByUserIDAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    // http://xamarinlobform.azurewebsites.net/api/WorkingLog/ByUserID?userID=user1&mode=user
                    var fooFullUrl = $"{FooAPIUrl}?userID={leaveAppFormByUserModel}&Mode=user";
                    #endregion
 
                    var fooJSON = JsonConvert.SerializeObject(leaveAppFormByUserModel);
                    // https://msdn.microsoft.com/zh-tw/library/system.net.http.stringcontent(v=vs.110).aspx
                    using (var fooContent = new StringContent(fooJSON, Encoding.UTF8, "application/json"))
                    {
                        response = await client.PostAsync(fooFullUrl, fooContent);
                    }
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                // 將 Payload 裡面的內容，反序列化為真實 .NET 要用到的資料
                                Items = JsonConvert.DeserializeObject<List<LeaveAppForm>>(fooAPIResult.Payload.ToString());
                                if (Items == null)
                                {
                                    Items = new List<LeaveAppForm>();
 
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = $"回傳的 API 內容不正確 : {fooAPIResult.Payload.ToString()}",
                                        Payload = null,
                                    };
                                }
                                else
                                {
                                    // 一般使用者與管理者所要查詢的請假單，將會存放在不同的檔案內
                                    await SaveAsync(leaveAppFormByUserModel.Mode);
                                }
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<LeaveAppForm>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            TokenFail = false,
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> GetAsync(int id)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.LeaveAppFormAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}/{id}";
                    #endregion
 
                    response = await client.GetAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<LeaveAppForm>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> PostAsync(LeaveAppForm leaveAppForm)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.LeaveAppFormAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    var fooJSON = JsonConvert.SerializeObject(leaveAppForm);
                    // https://msdn.microsoft.com/zh-tw/library/system.net.http.stringcontent(v=vs.110).aspx
                    using (var fooContent = new StringContent(fooJSON, Encoding.UTF8, "application/json"))
                    {
                        response = await client.PostAsync(fooFullUrl, fooContent);
                    }
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<LeaveAppForm>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> PutAsync(LeaveAppForm leaveAppForm)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.LeaveAppFormAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}";
                    #endregion
 
                    var fooJSON = JsonConvert.SerializeObject(leaveAppForm);
                    // https://msdn.microsoft.com/zh-tw/library/system.net.http.stringcontent(v=vs.110).aspx
                    using (var fooContent = new StringContent(fooJSON, Encoding.UTF8, "application/json"))
                    {
                        response = await client.PutAsync(fooFullUrl, fooContent);
                    }
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<LeaveAppForm>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    public async Task<APIResult> DeleteAsync(LeaveAppForm leaveAppForm)
    {
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    string FooAPIUrl = $"{MainHelper.LeaveAppFormAPIUrl}";
                    HttpResponseMessage response = null;
 
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
                    // 這裡是要存取 Azure Mobile 服務必須要指定的 Header
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
 
                    #region 傳入 Access Token
                    var fooSystemStatus = new SystemStatusRepository();
                    await fooSystemStatus.ReadAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                       fooSystemStatus.Item.AccessToken);
                    #endregion
 
                    #region  設定相關網址內容
                    var fooFullUrl = $"{FooAPIUrl}/{leaveAppForm.LeaveAppFormId}";
                    #endregion
 
                    response = await client.DeleteAsync(fooFullUrl);
                    #endregion
 
                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            #region 狀態碼為成功
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult.Success == true)
                            {
                                #region 讀取成功的回傳資料
                                #endregion
                            }
                            else
                            {
                                #region API 的狀態碼為 不成功
                                Items = new List<LeaveAppForm>();
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = fooAPIResult.Message,
                                    Payload = Items,
                                };
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region API 的狀態碼為 不成功
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = $"狀態碼：{response.StatusCode}{Environment.NewLine}{response.ReasonPhrase}",
                                Payload = Items,
                                TokenFail = response.StatusCode == HttpStatusCode.Unauthorized ? true : false
                            };
                            #endregion
                        }
                    }
                    else
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = "應用程式呼叫 API 發生異常",
                            Payload = null,
                        };
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    fooAPIResult = new APIResult
                    {
                        Success = false,
                        Message = ex.Message,
                        Payload = ex,
                    };
                }
            }
        }
 
        return fooAPIResult;
    }
 
    /// <summary>
    /// 將資料寫入到檔案內
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync(string Mode)
    {
        string data = JsonConvert.SerializeObject(Items);
        await StorageUtility.WriteToDataFileAsync("", MainHelper.資料主目錄, MainHelper.LeaveAppFormAPIName + Mode, data);
    }
 
    /// <summary>
    /// 將資料讀取出來
    /// </summary>
    /// <returns></returns>
    public async Task<List<LeaveAppForm>> ReadAsync(string Mode)
    {
        string data = "";
        data = await StorageUtility.ReadFromDataFileAsync("", MainHelper.資料主目錄, MainHelper.LeaveAppFormAPIName + Mode);
        Items = JsonConvert.DeserializeObject<List<LeaveAppForm>>(data);
        if (Items == null)
        {
            Items = new List<LeaveAppForm>();
        }
        return Items;
    }
}
```

# 確認專案可以執行

* 請分別在 Android / iOS / UWP 平台下，執行這個專案，看看能否正常執行。

## 修正 MainHelper 類別

* 在 `Helpers` 資料夾，打開 `MainHelper.cs`

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using Acr.UserDialogs;
using LOBForm.Models;
using LOBForm.Repositories;
using System.Threading;
using System.Threading.Tasks;
```

* 在這個類別中，加入底下兩個方法程式碼

![](Icons/csharp.png)

```csharp
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
```

# 問題研究

![](Icons/fa-question-circle30.png) 為什麼要在 MainHelper 類別中，宣告這麼多的字串常數與字串屬性呢？

![](Icons/fa-question-circle30.png) StorageUtility 這個支援類別是做甚麼用途的，我們為什麼需要它呢？

![](Icons/fa-question-circle30.png) StorageUtility 類別是否可以在不同行動作業系統(Android / iOS / UWP)下，順利正常的讀寫檔案呢？這些檔案會存在多久呢？

![](Icons/fa-question-circle30.png) 為什麼在使用 `HttpClient` 和 `HttpClientHandler` 物件的時候，將相關程式碼放在 `using` 區塊內？

![](Icons/fa-question-circle30.png) `HttpClientHandler` 好像沒有用到，我一定要使用他嗎？

![](Icons/fa-question-circle30.png) 為什麼這些 Repository 類別，裡面要存取 Web API 方法，都要設計使用非同步方式呢？ 若不使用非同步方式程式碼來設計，會有甚麼問題嗎？

![](Icons/fa-question-circle30.png) 我們要如何判斷，此次呼叫 Web API結果，是成功的？

![](Icons/fa-question-circle30.png) 每個儲存庫類別中，都會看到有這兩個方法 SaveAsync / ReadAsync，看起來程式碼都相當類似，有甚麼方式可以讓這些儲存體類別程式碼減少一些，但一樣會提供讀取與儲存檔案功能。

![](Icons/fa-question-circle30.png) 這些儲存庫若呼叫 Web API 成功，都會將這些結果寫入到手機裝置內，請問，這麼設計的理由是甚麼？

![](Icons/fa-question-circle30.png) 請從這個練習中，學習如何在 HttpClient 類別物件中，可以傳送 HTTP 標頭資訊？

![](Icons/fa-question-circle30.png) 在這些儲存庫類別中，當讀取了 HttpResponseMessage 物件之後，為什麼會這麼多的判斷邏輯程式碼？它主要目的是甚麼呢？

![](Icons/fa-question-circle30.png) 在 `LoginRepository` 儲存庫類別中，提供了兩種登入的方法，一個是使用 GET 方法(使用 HTTP 基本認證)，另外一個是使用 POST 方法，請試著了解在 Xamarin.Forms 如何撰寫這樣的程式碼？

![](Icons/fa-question-circle30.png) 在 LoginRepository 類別中，若經過呼叫登入驗證 Web API 成功之後，便會得接下來呼叫 Web API 要使用的存取權杖，我們需要將這個存取權杖記錄在手機記憶卡內，下次要呼叫 Web API 的時候，便可以取回這個存取權杖；因此，請嘗試了解這個練習範例中，使用了甚麼的技巧來做到這樣的需求。

![](Icons/fa-question-circle30.png) 當我們要呼叫 Web API 的時候，並且要提供存取權杖，各位可以參考 `WorkingLogRepository` / `LeaveAppFormRepository` 這兩個類別，搜尋關鍵字 `#region 傳入 Access Token` 就會看到這些程式嗎

![](Icons/fa-question-circle30.png) 若存取權杖有異常或者授權可用時間已經逾期，會不會造成這些儲存庫類別產生例外異常，進而App閃退嗎？

![](Icons/fa-question-circle30.png) 若要了解在 Xamarin.Forms 中，如何做到呼叫 CRUD 的 Web API，需要了解、參考這兩個類別程式碼 `WorkingLogRepository` / `LeaveAppFormRepository`

![](Icons/fa-question-circle30.png) 若存取權杖的有效期限已經到期，我是否可以修改手機的時間，將其往前調整，這樣，同樣的存取權杖否就可以使用了呢？




