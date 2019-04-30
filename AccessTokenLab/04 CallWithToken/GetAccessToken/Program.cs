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
