using Shared;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Helpers
{
    public class Store
    {

        public static class ConfessClass
        {
            public static async Task DeleteConfess(string guid, string token)
            {
                try
                {
                    string url = $"confess/delete?guid={guid}";
                    string content = await BaseClient.DeleteEntities(url, token);
                }
                catch (Exception ex)
                {
                    
                }
            }
            public static async Task CreateConfess(Confess confess, string token)
            {
                try
                {
                    string url = "confess/add";
                    string content = await BaseClient.PostEntities(url, JsonConvert.SerializeObject(confess), token);
                }
                catch (Exception ex)
                {
                    
                }
            }
            public static async Task UpdateConfess(Confess confess, string token)
            {
                try
                {
                    string url = "confess/update";
                    string content = await BaseClient.PutEntities(url, JsonConvert.SerializeObject(confess), token);
                }
                catch (Exception ex)
                {
                    
                }
            }

            public static async Task<List<ConfessLoader>> FetchConfessByCategory(string category, string token, string key)
            {
                try
                {
                    string url = $"confess/fetch/cat?key={key}&cat={category}";
                    string content = await BaseClient.GetEntities(url, token);
                    List<ConfessLoader> data = JsonConvert.DeserializeObject<List<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return new List<ConfessLoader>();
                }
            }

            public static async Task<List<ConfessLoader>> FetchMyConfessions(string token, string key)
            {
                try
                {
                    string url = $"confess/fetch?key={key}";
                    string content = await BaseClient.GetEntities(url, token);
                    List<ConfessLoader> data = JsonConvert.DeserializeObject<List<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return new List<ConfessLoader>();
                }
            }


            public static async Task<Confess> FetchOneConfessByGuid(string guid, string token)
            {
                try
                {
                    string url = $"confess/fetch/guid?guid={guid}";
                    string content = await BaseClient.GetEntities(url, token);
                    Confess data = JsonConvert.DeserializeObject<Confess>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return new Confess() { };
                }
            }
            public static async Task<ConfessLoader> FetchOneConfessLoaderByGuid(string guid, string token, string key)
            {
                try
                {
                    string url = $"confess/fetchloader/guid?guid={guid}&key={key}";
                    string content = await BaseClient.GetEntities(url, token);
                    ConfessLoader data = JsonConvert.DeserializeObject<ConfessLoader>(content);
                    return data;
                }
                catch (Exception ex)
                {

                    return new ConfessLoader() { };
                }
            }



            public static async Task<List<ConfessLoader>> FetchAllConfess(string token, string key)
            {
                try
                {
                    string url = $"confess/fetchall?key={key}";
                    string content = await BaseClient.GetEntities(url, token);
                    List<ConfessLoader> data = JsonConvert.DeserializeObject<List<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return new List<ConfessLoader>();
                }
            }
        }

        public static class CommentClass
        {
            public static async Task<ConfessLoader> DeleteComment(string guid, string ConfessGuid, string token, string key)
            {
                try
                {
                    string url = $"comment/delete?guid={guid}&confess={ConfessGuid}&key={key}";
                    string content = await BaseClient.DeleteEntities(url, token);
                    ConfessLoader data = JsonConvert.DeserializeObject<ConfessLoader>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return null;
                }
            }


            public static async Task<ConfessLoader> CreateComment(Comment comment, string confessGuid, string token, string key)
            {
                try
                {
                    string url = $"comment/add";
                    CommentPoster poster = new CommentPoster() {
                         Comment = comment, ConfessGuid =confessGuid, Key = key
                    };
                    string content = await BaseClient.PostEntities(url, JsonConvert.SerializeObject(poster), token);
                    ConfessLoader data = JsonConvert.DeserializeObject<ConfessLoader>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return null;
                }
            }

            public static async Task<List<CommentLoader>> FetchComment(string guid, string token, string key)
            {
                try
                {
                    string url = $"comment/fetch?guid={guid}&key={key}";
                    string content = await BaseClient.GetEntities(url, token);
                    List<CommentLoader> data = JsonConvert.DeserializeObject<List<CommentLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return new List<CommentLoader>() { };
                }
            }
        }

        public static class LikeClass
        {
            public static async Task<ConfessSender> Post(string guid, bool isComment, string confessguid, string token, string key)
            {
                try
                {
                    string url = $"like/add?guid={guid}&isComment={isComment}&key={key}&confess={confessguid}";
                    string content = await BaseClient.GetEntities(url, token);
                    var data = JsonConvert.DeserializeObject<ConfessSender>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return null;
                }
            }
        }

        public static class DislikeClass
        {
            public static async Task<ConfessSender> Post(string guid, bool isComment, string confessguid, string token, string key)
            {
                try
                {
                    string url = $"dislike/add?key={key}&guid={guid}&isComment={isComment}&confess={confessguid}";
                    string content = await BaseClient.GetEntities(url, token);
                    ConfessSender data = JsonConvert.DeserializeObject<ConfessSender>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    
                    return null;
                }
            }
        }
    }
}
