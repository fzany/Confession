using Newtonsoft.Json;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uwp.Helpers;
using Uwp.Models;

namespace Store.Helpers
{
    public class Online
    {
        public static class ConfessClass
        {
            public static async Task DeleteConfess(string guid)
            {
                try
                {
                    string url = $"confess/delete?guid={guid}";
                    string content = await BaseClient.DeleteEntities(url);
                }
                catch (Exception)
                {

                }
            }
            public static async Task CreateConfess(Confess confess)
            {
                try
                {
                    string url = "confess/add";
                    string content = await BaseClient.PostEntities(url, JsonConvert.SerializeObject(confess));
                }
                catch (Exception)
                {

                }
            }
            public static async Task UpdateConfess(Confess confess)
            {
                try
                {
                    string url = "confess/update";
                    string content = await BaseClient.PutEntities(url, JsonConvert.SerializeObject(confess));
                }
                catch (Exception)
                {

                }
            }

            public static async Task<List<ConfessLoader>> FetchConfessByCategory(string category)
            {
                try
                {
                    string url = $"confess/fetch/cat?key={Logic.GetKey()}&cat={category}";
                    string content = await BaseClient.GetEntities(url);
                    List<ConfessLoader> data = JsonConvert.DeserializeObject<List<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception)
                {

                    return new List<ConfessLoader>();
                }
            }

            public static async Task<List<ConfessLoader>> FetchMyConfessions()
            {
                try
                {
                    string url = $"confess/fetch?key={Logic.GetKey()}";
                    string content = await BaseClient.GetEntities(url);
                    List<ConfessLoader> data = JsonConvert.DeserializeObject<List<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception)
                {

                    return new List<ConfessLoader>();
                }
            }


            public static async Task<Confess> FetchOneConfessByGuid(string guid)
            {
                try
                {
                    string url = $"confess/fetch/guid?guid={guid}";
                    string content = await BaseClient.GetEntities(url);
                    Confess data = JsonConvert.DeserializeObject<Confess>(content);
                    return data;
                }
                catch (Exception)
                {

                    return new Confess() { };
                }
            }



            public static async Task<List<ConfessLoader>> FetchAllConfess()
            {
                try
                {
                    string url = $"confess/fetchall?key={Logic.GetKey()}";
                    string content = await BaseClient.GetEntities(url);
                    List<ConfessLoader> data = JsonConvert.DeserializeObject<List<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception)
                {

                    return new List<ConfessLoader>();
                }
            }
        }

        public static class CommentClass
        {
            public static async Task<ConfessLoader> DeleteComment(string guid, string ConfessGuid)
            {
                try
                {
                    string url = $"comment/delete?guid={guid}&confess={ConfessGuid}&key={Logic.GetKey()}";
                    string content = await BaseClient.DeleteEntities(url);
                    ConfessLoader data = JsonConvert.DeserializeObject<ConfessLoader>(content);
                    return data;
                }
                catch (Exception)
                {

                    return null;
                }
            }


            public static async Task<ConfessLoader> CreateComment(Comment comment, string confessGuid)
            {
                try
                {
                    string url = $"comment/add";
                    CommentPoster poster = new CommentPoster()
                    {
                        Comment = comment,
                        ConfessGuid = confessGuid,
                        Key = Logic.GetKey()
                    };
                    string content = await BaseClient.PostEntities(url, JsonConvert.SerializeObject(poster));
                    ConfessLoader data = JsonConvert.DeserializeObject<ConfessLoader>(content);
                    return data;
                }
                catch (Exception)
                {

                    return null;
                }
            }

            public static async Task<List<CommentLoader>> FetchComment(string guid)
            {
                try
                {
                    string url = $"comment/fetch?guid={guid}&key={Logic.GetKey()}";
                    string content = await BaseClient.GetEntities(url);
                    List<CommentLoader> data = JsonConvert.DeserializeObject<List<CommentLoader>>(content);
                    return data;
                }
                catch (Exception)
                {

                    return new List<CommentLoader>() { };
                }
            }
        }

        public static class LikeClass
        {
            public static async Task<ConfessSender> Post(string guid, bool isComment, string confessguid)
            {
                try
                {
                    string url = $"like/add?guid={guid}&isComment={isComment}&key={Logic.GetKey()}&confess={confessguid}";
                    string content = await BaseClient.GetEntities(url);
                    ConfessSender data = JsonConvert.DeserializeObject<ConfessSender>(content);
                    return data;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public static class DislikeClass
        {
            public static async Task<ConfessSender> Post(string guid, bool isComment, string confessguid)
            {
                try
                {
                    string url = $"dislike/add?key={Logic.GetKey()}&guid={guid}&isComment={isComment}&confess={confessguid}";
                    string content = await BaseClient.GetEntities(url);
                    ConfessSender data = JsonConvert.DeserializeObject<ConfessSender>(content);
                    return data;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

    }
}
