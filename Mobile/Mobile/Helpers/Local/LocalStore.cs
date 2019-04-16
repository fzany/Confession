using LiteDB;
using Microsoft.AppCenter.Crashes;
using Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Helpers.Local
{
    public class LocalStore
    {
        private static readonly LocalDataContext Local = new LocalDataContext();

        public static class Confession
        {
            public static void SaveLoaders(ObservableCollection<ConfessLoader> loaders)
            {
                ObservableCollection<ConfessLoader> insert_loaders = new ObservableCollection<ConfessLoader>();
                foreach (ConfessLoader load in loaders)
                {
                    if (Local.ConfessLoader.Exists(d => d.Id == load.Id))
                    {
                        //update such confession
                        Local.ConfessLoader.Update(load);
                    }
                    else
                    {
                        insert_loaders.Add(load);
                    }
                }
                if (insert_loaders.Count > 0)
                {
                    Local.ConfessLoader.InsertBulk(insert_loaders);
                }
            }

            public static ObservableCollection<ConfessLoader> FetchAllLoaders()
            {
                System.Collections.Generic.IEnumerable<ConfessLoader> result = Local.ConfessLoader.FindAll();
                return new ObservableCollection<ConfessLoader>(result);
            }

            public static ObservableCollection<ConfessLoader> FetchByCategory(string category)
            {
                System.Collections.Generic.IEnumerable<ConfessLoader> result = Local.ConfessLoader.Find(b => b.Category == category);
                return new ObservableCollection<ConfessLoader>(result);
            }
            public static async Task<ObservableCollection<ConfessLoader>> FetchMine()
            {
                string mykey = await Logic.GetKey();
                System.Collections.Generic.IEnumerable<ConfessLoader> result = Local.ConfessLoader.Find(f => f.Owner_Guid == mykey);
                return new ObservableCollection<ConfessLoader>(result);
            }

            public static void DeleteLoader(string guid)
            {
                Local.ConfessLoader.Delete(g => g.Guid == guid);
            }

            internal static void SaveLoader(ConfessLoader incomingConfession)
            {
                if (Local.ConfessLoader.Exists(d => d.Id == incomingConfession.Id))
                {
                    Local.ConfessLoader.Update(incomingConfession);
                }
                else
                {
                    Local.ConfessLoader.Insert(incomingConfession);
                }
            }

            internal static void UpdateLoader(ConfessLoader incomingConfession)
            {
                Local.ConfessLoader.Update(incomingConfession);
            }
        }

        public static class Comment
        {
            public static void SaveLoaders(ObservableCollection<CommentLoader> loaders)
            {
                ObservableCollection<CommentLoader> insert_loaders = new ObservableCollection<CommentLoader>();
                foreach (CommentLoader load in loaders)
                {
                    if (Local.CommentLoader.Exists(d => d.Id == load.Id))
                    {
                        //update such confession
                        Local.CommentLoader.Update(load);
                    }
                    else
                    {
                        insert_loaders.Add(load);
                    }
                }
                if (insert_loaders.Count > 0)
                {
                    Local.CommentLoader.InsertBulk(insert_loaders);
                }
            }

            public static ObservableCollection<CommentLoader> FetchByConfessGuid(string guid)
            {
                System.Collections.Generic.IEnumerable<CommentLoader> result = Local.CommentLoader.Find(b => b.Confess_Guid == guid);
                return new ObservableCollection<CommentLoader>(result);
            }

            internal static void SaveLoader(CommentLoader load)
            {
                if (!Local.CommentLoader.Exists(d => d.Id == load.Id))
                {
                    Local.CommentLoader.Insert(load);
                }
            }

            internal static void UpdateLoader(CommentLoader replacer)
            {
                Local.CommentLoader.Update(replacer);
            }

            internal static void DeleteLoader(string message)
            {
                Local.CommentLoader.Delete(d=>d.Guid == message);
            }
        }

        public static class Chat
        {
            //continue
            public static void SaveLoaders(ObservableCollection<ChatLoader> loaders)
            {
                foreach (ChatLoader load in loaders)
                {
                    if (Local.ChatLoader.Exists(d => d.ChatId == load.ChatId))
                    {
                        //update such confession
                        Local.ChatLoader.Update(load);
                    }
                    else
                    {
                        load.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                        Local.ChatLoader.Insert(load);
                    }
                }

            }

            public static void SaveLoader(ChatLoader loader)
            {

                try
                {
                    if (Local.ChatLoader.Exists(d => d.ChatId == loader.ChatId))
                    {
                        //update such confession
                        Local.ChatLoader.Update(loader);
                    }
                    else
                    {
                        loader.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                        Local.ChatLoader.Insert(loader);
                    }
                }
                catch (System.Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            }



            internal static List<Models.Chat> FetchQueuedChats()
            {
                return Local.Chat.FindAll().ToList();
            }

            public static ObservableCollection<ChatLoader> FetchByRoomID(string roomID)
            {
                System.Collections.Generic.IEnumerable<ChatLoader> result = Local.ChatLoader.Find(b => b.Room_ID == roomID);
                return new ObservableCollection<ChatLoader>(result);
            }

            public static void DeleteLoader(string guid)
            {
                Local.ChatLoader.Delete(g => g.ChatId == guid);
            }

            internal static void SavePending(Models.Chat new_send)
            {
                try
                {
                    Local.Chat.Insert(new_send);
                }
                catch (System.Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            }

            internal static void CancelPend(string id)
            {
                Local.Chat.Delete(d => d.Id == id);
            }


        }

        public static class ChatRoom
        {
            public static ObservableCollection<ChatRoomLoader> GetAllRooms()
            {
                return new ObservableCollection<ChatRoomLoader>(Local.ChatRoomLoader.FindAll());
            }
            public static void SaveLoaders(ObservableCollection<ChatRoomLoader> loaders)
            {
                ObservableCollection<ChatRoomLoader> insert_loaders = new ObservableCollection<ChatRoomLoader>();
                try
                {
                    foreach (ChatRoomLoader load in loaders)
                    {
                        if (Local.ChatRoomLoader.Exists(d => d.Id == load.Id))
                        {
                            //update such confession
                            Local.ChatRoomLoader.Update(load);
                        }
                        else
                        {
                            insert_loaders.Add(load);
                        }
                    }
                    if (insert_loaders.Count > 0)
                    {
                        Local.ChatRoomLoader.InsertBulk(insert_loaders);
                    }
                }
                catch (System.Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            }

            internal static void UpdateMembership(string roomId, string count)
            {
                if (Local.ChatRoomLoader.Exists(d => d.Id == roomId))
                {
                    ChatRoomLoader room = Local.ChatRoomLoader.FindOne(d => d.Id == roomId);
                    room.MembersCount = count;
                    Local.ChatRoomLoader.Update(room);
                }
            }
        }

        public static class Generic
        {
            public static void DropCollections()
            {
                try
                {
                    List<BsonDocument> allconfe = Local.ConfessRaw.FindAll().ToList();
                    foreach (BsonDocument conff in allconfe)
                    {
                        Local.ConfessRaw.Delete(conff);
                    }
                    List<BsonDocument> allcomm = Local.CommentRaw.FindAll().ToList();
                    foreach (BsonDocument comm in allcomm)
                    {
                        Local.CommentRaw.Delete(comm);
                    }

                    List<BsonDocument> allchat = Local.ChatLoaderRaw.FindAll().ToList();
                    foreach (BsonDocument chat in allchat)
                    {
                        Local.ChatLoaderRaw.Delete(chat);
                    }

                    List<BsonDocument> allrooms = Local.ChatRoomRaw.FindAll().ToList();
                    foreach (BsonDocument room in allrooms)
                    {
                        Local.ChatRoomRaw.Delete(room);
                    }

                    List<BsonDocument> allchats = Local.ChatRaw.FindAll().ToList();
                    foreach (BsonDocument chat in allchats)
                    {
                        Local.ChatRaw.Delete(chat);
                    }


                }
                catch (System.Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }

            }
            public static double GetDatabaseSize()
            {
                try
                {
                    double value = 0;
                    value += Encoding.Unicode.GetByteCount(Local.ConfessRaw.FindAll().ToList().ToString());
                    value += Encoding.Unicode.GetByteCount(Local.CommentRaw.FindAll().ToList().ToString());
                    value += Encoding.Unicode.GetByteCount(Local.ChatLoaderRaw.FindAll().ToList().ToString());
                    value += Encoding.Unicode.GetByteCount(Local.ChatRoomRaw.FindAll().ToList().ToString());
                    value += Encoding.Unicode.GetByteCount(Local.ChatRaw.FindAll().ToList().ToString());
                    return value;
                }
                catch (System.Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("My custom event");

                    return 0;
                }
            }
        }
    }
}
