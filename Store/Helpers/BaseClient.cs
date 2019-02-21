using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Uwp.Helpers;

namespace Store.Helpers
{
    public class BaseClient
    {
        internal static async Task<string> PostEntities(string urlLink, string content)
        {
            HttpClient httpClient = HttpConnection.CreateClient(Logic.GetToken());
            HttpResponseMessage httpResponse = await httpClient.PostAsync(urlLink, new StringContent(content, Encoding.UTF8, Constants.ContentTypeHeaderJson));
            return await httpResponse.Content.ReadAsStringAsync();
        }
        internal static async Task<string> GetEntities(string urlLink)
        {
            HttpClient client = HttpConnection.CreateClient(Logic.GetToken());
            Task<HttpResponseMessage> response = client.GetAsync(urlLink);
            return await response.Result.Content.ReadAsStringAsync();
        }

        internal static async Task<string> GetEntities(string urlLink, string key)
        {
            HttpClient client = HttpConnection.CreateClient(key);
            Task<HttpResponseMessage> response = client.GetAsync(urlLink);
            return await response.Result.Content.ReadAsStringAsync();
        }
        internal static async Task<string> Get(string url)
        {
            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            //Add a user-agent header to the GET request. 
            Windows.Web.Http.Headers.HttpRequestHeaderCollection headers = httpClient.DefaultRequestHeaders;
            headers.Add("cache-control", "no-cache");
            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }
            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri(Constants.BaseURL+url);

            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                return httpResponseBody;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        internal static async Task<string> DeleteEntities(string urlLink)
        {
            HttpClient client = HttpConnection.CreateClient(Logic.GetToken());
            Task<HttpResponseMessage> response = client.DeleteAsync(urlLink);
            return await response.Result.Content.ReadAsStringAsync();
        }
        internal static async Task<string> PutEntities(string urlLink, string content)
        {
            HttpClient client = HttpConnection.CreateClient(Logic.GetToken());
            HttpResponseMessage response = await client.PutAsync(urlLink, new StringContent(content, Encoding.UTF8, Constants.ContentTypeHeaderJson));
            return await response.Content.ReadAsStringAsync();
        }
    }
}
