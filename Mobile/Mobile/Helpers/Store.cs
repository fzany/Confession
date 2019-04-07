using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Mobile.Helpers.Local;
using Mobile.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Mobile.Helpers
{
    public class Store
    {
        public static class GenericClass
        {
            internal static async Task<string> GetName()
            {
                try
                {
                    string url = $"generic/getaname";
                    string content = await BaseClient.GetEntities(url);
                    string data = JsonConvert.DeserializeObject<string>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return "";
                }
            }
        }

        public static class UserClass
        {
            internal static async Task<UserData> Get()
            {
                try
                {
                    Guid? installId = await AppCenter.GetInstallIdAsync();
                    string url = $"user/fetch?appcenter={installId.Value.ToString()}";
                    string content = await BaseClient.GetEntities(url);
                    UserData data = JsonConvert.DeserializeObject<UserData>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new UserData() { };
                }
            }
            public static async Task Add()
            {
                //fetch the current user. 
                UserData user_data = await Store.UserClass.Get();
                //A first timer would return null
                Guid? installId = await AppCenter.GetInstallIdAsync();
                string currentkey = await Logic.GetKey();
                if (user_data == null || string.IsNullOrEmpty(user_data.AppCenterID) || user_data.Logger == null || user_data.Key == null)
                {
                    //means a new user; 
                    //Add user to the db
                    user_data = new UserData
                    {
                        AppCenterID = installId.Value.ToString(),
                        Biometry = false,
                        ChatRoomNotification = true,
                        CommentNotification = true,
                        Logger = Logic.GetDeviceInformation()
                    };
                    //specially treat keys to prevent loss. 
                    if (user_data.Key == null || user_data.Key.Count == 0)
                    {
                        user_data.Key = new List<string>() { currentkey };
                    }
                    else
                    {
                        if (!user_data.Key.Contains(currentkey))
                        {
                            user_data.Key.Insert(0, currentkey);
                        }
                    }
                    try
                    {
                        string url = "user/add";
                        await BaseClient.PostEntities(url, JsonConvert.SerializeObject(user_data));
                        // await Logic.CreateLogged();
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                }
                else
                {
                    //An existing user would have all data complete but things might have changed
                    //Note that anything that change like key and token would have been re-created
                    //appcenter id too can change. 
                }
            }
            public static async Task Update(UserData user)
            {
                try
                {
                    string url = "user/update";
                    string content = await BaseClient.PutEntities(url, JsonConvert.SerializeObject(user));
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
        }

        public static class ChatClass
        {
            internal static async Task JoinRoom(string roomID)
            {
                try
                {
                    if (string.IsNullOrEmpty(roomID))
                    {
                        roomID = await Logic.GetRoomID();
                    }

                    string url = $"chat/join?id={roomID}";
                    await BaseClient.GetEntities(url);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }

            internal static async Task LeaveRoom(string roomID)
            {
                try
                {
                    if (string.IsNullOrEmpty(roomID))
                    {
                        roomID = await Logic.GetRoomID();
                    }

                    string url = $"chat/leave?id={roomID}";
                    await BaseClient.GetEntities(url);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
            internal static async Task<ObservableCollection<ChatRoomLoader>> Rooms()
            {
                try
                {
                    string url = $"chat/fetchrooms";
                    string content = await BaseClient.GetEntities(url);
                    ObservableCollection<ChatRoomLoader> data = JsonConvert.DeserializeObject<ObservableCollection<ChatRoomLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new ObservableCollection<ChatRoomLoader>() { };
                }
            }

            internal static async Task<ObservableCollection<ChatLoader>> ChatsByRoom(string roomID)
            {
                try
                {
                    if (string.IsNullOrEmpty(roomID))
                    {
                        roomID = await Logic.GetRoomID();
                    }

                    string url = $"chat/fetchchats?roomID={roomID}";
                    string content = await BaseClient.GetEntities(url);
                    ObservableCollection<ChatLoader> data = JsonConvert.DeserializeObject<ObservableCollection<ChatLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new ObservableCollection<ChatLoader>() { };
                }
            }

            public static async Task Add(Chat chat)
            {
                try
                {
                    string url = "chat/add";
                    string content = await BaseClient.PostEntities(url, JsonConvert.SerializeObject(chat));
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
            public static async Task Update(Chat chat)
            {
                try
                {
                    string url = "chat/update";
                    string content = await BaseClient.PutEntities(url, JsonConvert.SerializeObject(chat));
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }


            public static async Task Delete(string id)
            {
                try
                {
                    string url = $"chat/delete?id={id}";
                    string content = await BaseClient.DeleteEntities(url);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }

        }

        public static class ConfessClass
        {
            public static async Task DeleteConfess(string guid)
            {
                try
                {
                    string url = $"confess/delete?guid={guid}";
                    string content = await BaseClient.DeleteEntities(url);
                    LocalStore.Confession.DeleteLoader(guid);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
            public static async Task CreateConfess(Confess confess)
            {
                try
                {
                    string url = "confess/add";
                    string content = await BaseClient.PostEntities(url, JsonConvert.SerializeObject(confess));
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
            public static async Task UpdateConfess(Confess confess)
            {
                try
                {
                    string url = "confess/update";
                    string content = await BaseClient.PutEntities(url, JsonConvert.SerializeObject(confess));
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }

            public static async Task<ObservableCollection<ConfessLoader>> FetchConfessByCategory(string category)
            {
                try
                {
                    string url = $"confess/fetch/cat?key={await Logic.GetKey()}&cat={category}";
                    string content = await BaseClient.GetEntities(url);
                    ObservableCollection<ConfessLoader> data = JsonConvert.DeserializeObject<ObservableCollection<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new ObservableCollection<ConfessLoader>();
                }
            }

            public static async Task<ObservableCollection<ConfessLoader>> FetchMyConfessions()
            {
                try
                {
                    string url = $"confess/fetch?key={await Logic.GetKey()}";
                    string content = await BaseClient.GetEntities(url);
                    ObservableCollection<ConfessLoader> data = JsonConvert.DeserializeObject<ObservableCollection<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new ObservableCollection<ConfessLoader>();
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
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new Confess() { };
                }
            }



            public static async Task<ObservableCollection<ConfessLoader>> FetchAllConfess()
            {
                try
                {
                    string url = $"confess/fetchall?key={await Logic.GetKey()}";
                    string content = await BaseClient.GetEntities(url);
                    ObservableCollection<ConfessLoader> data = JsonConvert.DeserializeObject<ObservableCollection<ConfessLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new ObservableCollection<ConfessLoader>();
                }
            }
        }

        public static class SettingClass
        {
            internal static async Task<Setting> GetSettings()
            {
                try
                {
                    string url = $"setting/fetch";
                    string content = await BaseClient.GetEntities(url);
                    Setting data = JsonConvert.DeserializeObject<Setting>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new Setting() { };
                }
            }
        }

        public static class CommentClass
        {
            public static async Task<ConfessLoader> DeleteComment(string guid, string ConfessGuid)
            {
                try
                {
                    string url = $"comment/delete?guid={guid}&confess={ConfessGuid}&key={await Logic.GetKey()}";
                    string content = await BaseClient.DeleteEntities(url);
                    ConfessLoader data = JsonConvert.DeserializeObject<ConfessLoader>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
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
                        Key = await Logic.GetKey()
                    };
                    string content = await BaseClient.PostEntities(url, JsonConvert.SerializeObject(poster));
                    ConfessLoader data = JsonConvert.DeserializeObject<ConfessLoader>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return null;
                }
            }

            public static async Task<ObservableCollection<CommentLoader>> FetchComment(string guid)
            {
                try
                {
                    string url = $"comment/fetch?guid={guid}&key={await Logic.GetKey()}";
                    string content = await BaseClient.GetEntities(url);
                    ObservableCollection<CommentLoader> data = JsonConvert.DeserializeObject<ObservableCollection<CommentLoader>>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return new ObservableCollection<CommentLoader>() { };
                }
            }
        }

        public static class LikeClass
        {
            public static async Task<ConfessSender> Post(string guid, bool isComment, string confessguid)
            {
                try
                {
                    string url = $"like/add?guid={guid}&isComment={isComment}&key={await Logic.GetKey()}&confess={confessguid}";
                    string content = await BaseClient.GetEntities(url);
                    ConfessSender data = JsonConvert.DeserializeObject<ConfessSender>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
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
                    string url = $"dislike/add?key={await Logic.GetKey()}&guid={guid}&isComment={isComment}&confess={confessguid}";
                    string content = await BaseClient.GetEntities(url);
                    ConfessSender data = JsonConvert.DeserializeObject<ConfessSender>(content);
                    return data;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return null;
                }
            }
        }
    }
}
