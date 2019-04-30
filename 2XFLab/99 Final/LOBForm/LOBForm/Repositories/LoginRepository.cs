using LOBForm.Helpers;
using LOBForm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LOBForm.Repositories
{
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
}
