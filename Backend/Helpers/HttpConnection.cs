using Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Backend.Helpers
{
    public static class HttpConnection
    {/// <summary>
     /// Initialize a new HttpClient with Security, content and cache configurations. 
     /// </summary>
     /// <param name="secretKey"></param>
     /// <returns></returns>
        public static HttpClient CreateClient()
        {
            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var client = new HttpClient()
            {
                BaseAddress = new Uri(Constants.AppCenterBaseURL)
            };


            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(Constants.ContentTypeHeaderJson));

            client.DefaultRequestHeaders.Add("cache-control", "no-cache");

            client.DefaultRequestHeaders.Add(Constants.XAPIToken, "87c29567a23ea1c279966a41e6aebd303cd6db0a");

          //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.XAPIToken, "");

            return client;
        }
    }
}
