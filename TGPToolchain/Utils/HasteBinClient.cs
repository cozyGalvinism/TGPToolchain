using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TGPToolchain.Utils
{
    public class HasteBinClient
    {
        private static HttpClient _httpClient;
        private string _baseUrl;
	
        static HasteBinClient()
        {
            _httpClient = new HttpClient();
        }

        public HasteBinClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
	
        public async Task<HasteBinResult> Post(string content)
        {
            string fullUrl = _baseUrl;
            if (!fullUrl.EndsWith("/"))
            {
                fullUrl += "/";
            }
            string postUrl = $"{fullUrl}documents";

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(postUrl));
            request.Content = new StringContent(content);
            HttpResponseMessage result = await _httpClient.SendAsync(request);
            string contentStr = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                HasteBinResult? hasteBinResult = JsonConvert.DeserializeObject<HasteBinResult>(contentStr);

                if (hasteBinResult?.Key == null)
                    return new HasteBinResult()
                    {
                        FullUrl = fullUrl,
                        IsSuccess = false,
                        StatusCode = (int) result.StatusCode,
                        Error = contentStr
                    };
                hasteBinResult.FullUrl = $"{fullUrl}{hasteBinResult.Key}";
                hasteBinResult.IsSuccess = true;
                hasteBinResult.StatusCode = 200;
                hasteBinResult.Error = null;
                return hasteBinResult;
            }

            return new HasteBinResult()
            {
                FullUrl = fullUrl,
                IsSuccess = false,
                StatusCode = (int) result.StatusCode,
                Error = contentStr
            };
        }
    }
    
    public class HasteBinResult
    {
        public string? Key { get; set; }
        public string? FullUrl { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string? Error { get; set; }
    }
}