using Microsoft.AppCenter.Crashes;
using Mobile.Models;
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
                    if (Local.ConfessLoader.Exists(d => d.Guid == load.Guid))
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
                if (Local.ConfessLoader.Exists(d => d.Guid == incomingConfession.Guid))
                {
                    Local.ConfessLoader.Update(incomingConfession);
                }
                else
                {
                    Local.ConfessLoader.Insert(incomingConfession);
                }
            }
        }

        public static class Comment
        {
            public static void SaveLoaders(ObservableCollection<CommentLoader> loaders)
            {
                ObservableCollection<CommentLoader> insert_loaders = new ObservableCollection<CommentLoader>();
                foreach (CommentLoader load in loaders)
                {
                    if (Local.CommentLoader.Exists(d => d.Guid == load.Guid))
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
        }

        public static class Chat
        {
            //continue
            public static void SaveLoaders(ObservableCollection<ChatLoader> loaders)
            {
                ObservableCollection<ChatLoader> insert_loaders = new ObservableCollection<ChatLoader>();
                foreach (ChatLoader load in loaders)
                {
                    if (Local.ChatLoader.Exists(d => d.ChatId == load.ChatId))
                    {
                        //update such confession
                        Local.ChatLoader.Update(load);
                    }
                    else
                    {
                        insert_loaders.Add(load);
                    }
                }
                if (insert_loaders.Count > 0)
                {
                    Local.ChatLoader.InsertBulk(insert_loaders);
                }
            }

            public static void SaveLoader(ChatLoader loader)
            {

                if (Local.ChatLoader.Exists(d => d.ChatId == loader.ChatId))
                {
                    //update such confession
                    Local.ChatLoader.Update(loader);
                }
                else
                {
                    Local.ChatLoader.Insert(loader);
                }
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

        }

        public static class Generic
        {
            public static void DropCollections()
            {
                try
                {
                    Local.ConfessLoader.Delete(f => f.Id.Length > 0);
                    Local.CommentLoader.Delete(f => f.Id.Length > 0);
                    Local.ChatLoader.Delete(f => f.Id.Length > 0);
                    Local.ChatRoomLoader.Delete(f => f.Id.Length > 0);
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
                    value += Encoding.Unicode.GetByteCount(Local.ConfessLoader.FindAll().ToList().ToString());
                    value += Encoding.Unicode.GetByteCount(Local.CommentLoader.FindAll().ToList().ToString());
                    value += Encoding.Unicode.GetByteCount(Local.ChatLoader.FindAll().ToList().ToString());
                    value += Encoding.Unicode.GetByteCount(Local.ChatRoomLoader.FindAll().ToList().ToString());
                    return value;
                }
                catch (System.Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                    return 0;
                }
            }
        }
    }
}
