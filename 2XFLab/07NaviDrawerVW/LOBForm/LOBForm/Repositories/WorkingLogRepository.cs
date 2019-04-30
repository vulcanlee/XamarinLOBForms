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
}
