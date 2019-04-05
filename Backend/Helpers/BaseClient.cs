using Shared;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Helpers
{
    public class BaseClient
    {
        internal static async Task<string> PostEntities(string urlLink, string content)
        {
            HttpClient httpClient = HttpConnection.CreateClient();
            HttpResponseMessage httpResponse = await httpClient.PostAsync(urlLink, new StringContent(content, Encoding.UTF8, Constants.ContentTypeHeaderJson)).ConfigureAwait(false);
            return await httpResponse.Content.ReadAsStringAsync();
        }
        internal static async Task<string> GetEntities(string urlLink)
        {
            HttpClient client = HttpConnection.CreateClient();
            Task<HttpResponseMessage> response = client.GetAsync(urlLink);
            return await response.Result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
      
        //internal static async Task<string> DeleteEntities(string urlLink)
        //{
        //    HttpClient client = HttpConnection.CreateClient();
        //    Task<HttpResponseMessage> response = client.DeleteAsync(urlLink);
        //    return await response.Result.Content.ReadAsStringAsync().ConfigureAwait(false);
        //}
        //internal static async Task<string> PutEntities(string urlLink, string content)
        //{
        //    HttpClient client = HttpConnection.CreateClient();
        //    HttpResponseMessage response = await client.PutAsync(urlLink, new StringContent(content, Encoding.UTF8, Constants.ContentTypeHeaderJson)).ConfigureAwait(false);
        //    return await response.Content.ReadAsStringAsync();
        //}
    }
}
