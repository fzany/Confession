using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Website.Helpers
{
    public class BaseClient
    {
        internal static async Task<string> PostEntities(string urlLink, string content, string token)
        {
            HttpClient httpClient = HttpConnection.CreateClient(token);
            HttpResponseMessage httpResponse = await httpClient.PostAsync(urlLink, new StringContent(content, Encoding.UTF8, Constants.ContentTypeHeaderJson)).ConfigureAwait(false);
            return await httpResponse.Content.ReadAsStringAsync();
        }
        internal static async Task<string> GetEntities(string urlLink, string token)
        {
            HttpClient client = HttpConnection.CreateClient(token);
            Task<HttpResponseMessage> response = client.GetAsync(urlLink);
            return await response.Result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
       
        internal static async Task<string> DeleteEntities(string urlLink, string token)
        {
            HttpClient client = HttpConnection.CreateClient(token);
            Task<HttpResponseMessage> response = client.DeleteAsync(urlLink);
            return await response.Result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        internal static async Task<string> PutEntities(string urlLink, string content, string token)
        {
            HttpClient client = HttpConnection.CreateClient(token);
            HttpResponseMessage response = await client.PutAsync(urlLink, new StringContent(content, Encoding.UTF8, Constants.ContentTypeHeaderJson)).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
