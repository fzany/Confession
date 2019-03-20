using MongoDB.Driver;
using Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Backend.Helpers
{
    public class Store
    {
        private static readonly DataContext context = new DataContext();
        private static readonly DataContextLite contextLite = new DataContextLite();

        public static class SettingsClass
        {
            internal static List<string> GetCategories()
            {
                List<string> cats = new List<string>();
                IEnumerable<Confess> confesses = contextLite.Confess.FindAll();
                foreach (Confess dat in confesses)
                {
                    if (!cats.Contains(dat.Category))
                    {
                        cats.Add(dat.Category);
                    }
                }
                return cats;
            }

            internal static void CreateLog(Logger data)
            {
                contextLite.Logger.Insert(data);
            }
        }
        #region Migrate
        public static class Migrate
        {
            public static void ClearLiteDB()
            {
                List<Confess> allconfess = FetchLite.FetchConfess();
                foreach (Confess data in allconfess)
                {
                    contextLite.Confess.Delete(d => d.Guid == data.Guid);
                }
            }
            public static class Create
            {
                public static void CreateConfess(List<Confess> Confess)
                {
                    contextLite.Confess.InsertBulk(Confess);
                }
                public static void CreateConfess(Confess Confess)
                {
                    contextLite.Confess.Insert(Confess);
                }
                public static void CreateSeen(List<Seen> Seen)
                {
                    contextLite.Seen.InsertBulk(Seen);
                }
                public static void CreateLikes(List<Likes> Likes)
                {
                    contextLite.Likes.InsertBulk(Likes);
                }
                public static void CreateComment(List<Comment> Comment)
                {
                    contextLite.Comment.InsertBulk(Comment);
                }
                public static void CreateDislikes(List<Dislikes> Dislikes)
                {
                    contextLite.Dislikes.InsertBulk(Dislikes);
                }
                public static void CreateUser(List<User> User)
                {
                    contextLite.User.InsertBulk(User);
                }
            }

            public static class Fetch
            {
                public static List<Confess> FetchConfess()
                {
                    FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                    FilterDefinition<Confess> empty = builder.Empty;
                    IFindFluent<Confess, Confess> cursor = context.Confess.Find(empty);
                    return cursor.ToList();
                }
                public static List<Seen> FetchSeen()
                {
                    FilterDefinitionBuilder<Seen> builder = Builders<Seen>.Filter;
                    FilterDefinition<Seen> empty = builder.Empty;
                    IFindFluent<Seen, Seen> cursor = context.Seen.Find(empty);
                    return cursor.ToList();
                }
                public static List<Likes> FetchLikes()
                {
                    FilterDefinitionBuilder<Likes> builder = Builders<Likes>.Filter;
                    FilterDefinition<Likes> empty = builder.Empty;
                    IFindFluent<Likes, Likes> cursor = context.Likes.Find(empty);
                    return cursor.ToList();
                }
                public static List<Comment> FetchComment()
                {
                    FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                    FilterDefinition<Comment> empty = builder.Empty;
                    IFindFluent<Comment, Comment> cursor = context.Comment.Find(empty);
                    return cursor.ToList();
                }
                public static List<Dislikes> FetchDislikes()
                {
                    FilterDefinitionBuilder<Dislikes> builder = Builders<Dislikes>.Filter;
                    FilterDefinition<Dislikes> empty = builder.Empty;
                    IFindFluent<Dislikes, Dislikes> cursor = context.Dislikes.Find(empty);
                    return cursor.ToList();
                }
                public static List<User> FetchUser()
                {
                    FilterDefinitionBuilder<User> builder = Builders<User>.Filter;
                    FilterDefinition<User> empty = builder.Empty;
                    IFindFluent<User, User> cursor = context.User.Find(empty);
                    return cursor.ToList();
                }
            }
            public static class FetchLite
            {
                public static List<Confess> FetchConfess()
                {
                    return contextLite.Confess.FindAll().ToList();
                }
                public static List<Seen> FetchSeen()
                {
                    return contextLite.Seen.FindAll().ToList();

                }
                public static List<Likes> FetchLikes()
                {
                    return contextLite.Likes.FindAll().ToList();

                }
                public static List<Comment> FetchComment()
                {
                    return contextLite.Comment.FindAll().ToList();
                }
                public static List<Dislikes> FetchDislikes()
                {
                    return contextLite.Dislikes.FindAll().ToList();
                }
                public static List<User> FetchUser()
                {
                    return contextLite.User.FindAll().ToList();
                }

                public static List<Logger> FetchLogger()
                {
                    return contextLite.Logger.FindAll().ToList();
                }
            }


            public class MigrateToMongo
            {
                //fetch all documents
                public static Migrator LoadMigration()
                {
                    Migrator migrator = new Migrator();
                    List<object> Lister = new List<object>();
                    Lister.AddRange(FetchLite.FetchConfess());
                    Lister.AddRange(FetchLite.FetchComment());
                    Lister.AddRange(FetchLite.FetchDislikes());
                    Lister.AddRange(FetchLite.FetchLikes());
                    Lister.AddRange(FetchLite.FetchLogger());
                    Lister.AddRange(FetchLite.FetchSeen());
                    Lister.AddRange(FetchLite.FetchUser());
                    migrator.Lister = Lister;
                    try
                    {
                        context.Logger.InsertOne(migrator);

                    }
                    catch (Exception)
                    {


                    }
                    return migrator;
                }


            }

        }
        #endregion
        public static class ConfessClass
        {
            public static void DeleteConfess(string guid)
            {
                contextLite.Confess.Delete(d => d.Guid == guid);

                //FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                //FilterDefinition<Confess> idFilter = builder.Eq(r => r.Guid, guid);
                //context.Confess.DeleteOne(idFilter);

                //delete all comment for that confession.
                CommentClass.DeleteCommentWithConfessGuid(guid);
            }
            public static void CreateConfess(Confess confess)
            {
                confess.Id = Logical.Setter(confess.Id);
                contextLite.Confess.Insert(confess);
                //context.Confess.InsertOne(confess);
            }
            public static void UpdateConfess(Confess incoming)
            {
                Confess confes = contextLite.Confess.FindOne(d => d.Guid == incoming.Guid);
                confes.Title = incoming.Title;
                confes.Body = incoming.Body;
                confes.Category = incoming.Category;
                contextLite.Confess.Update(confes);

                // FilterDefinition<Confess> filter = Builders<Confess>.Filter.Eq(u => u.Guid, confess.Guid);
                // context.Confess.ReplaceOne(filter, confess);
            }

            public static List<ConfessLoader> FetchConfessByCategory(string category, string key)
            {
                List<Confess> list = contextLite.Confess.Find(d => d.Category == category).ToList();

                //FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                //FilterDefinition<Confess> idFilter = builder.Eq(e => e.Category, category);

                //IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find All
                return GetConfessLoader(list, key);
            }

            public static List<ConfessLoader> FetchMyConfessions(string key)
            {
                List<Confess> list = contextLite.Confess.Find(d => d.Owner_Guid == key).ToList();

                //FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                //FilterDefinition<Confess> idFilter = builder.Eq(e => e.Owner_Guid, key);

                //IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find All
                return GetConfessLoader(list, key);
            }
            public static ConfessLoader FetchOneConfessLoader(string guid, string key)
            {
                Confess data = FetchOneConfessByGuid(guid);
                //report seen
                if (data.Owner_Guid != key)
                {
                    SeenClass.Post(guid, key);
                }
                // contextLite.Confess.FindOne(d => d.Guid == guid);

                //FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                //FilterDefinition<Confess> idFilter = builder.Eq(e => e.Guid, guid);

                //IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find All
                return GetConfessLoader(data, key);
            }

            public static Confess FetchOneConfessByGuid(string guid)
            {
                return contextLite.Confess.FindOne(d => d.Guid == guid);
                //FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                //FilterDefinition<Confess> idFilter = builder.Eq(e => e.Guid, guid);

                //IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                //// Find One
                //return cursor.FirstOrDefault();
            }

            public static List<ConfessLoader> FetchAllConfess(string key)
            {
                try
                {
                    List<Confess> data = contextLite.Confess.FindAll().ToList();
                    //FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                    //FilterDefinition<Confess> empty = builder.Empty;
                    //IFindFluent<Confess, Confess> cursor = context.Confess.Find(empty);
                    return GetConfessLoader(data, key);
                }
                catch (Exception)
                {
                    return new List<ConfessLoader>();
                }
            }

            private static List<ConfessLoader> GetConfessLoader(List<Confess> list, string key)
            {
                List<ConfessLoader> loaders = new List<ConfessLoader>();
                foreach (Confess dt in list)
                {
                    ConfessLoader loader = new ConfessLoader
                    {
                        Body = dt.Body,
                        Category = dt.Category,
                        Date = Shared.TimeAgo.Ago(dt.Date),// $"{dt.Date.ToLongDateString()} {dt.Date.ToShortTimeString()}",
                        DateReal = dt.Date,
                        DisLikes = DislikeClass.GetCount(dt.Guid, false),
                        Likes = LikeClass.GetCount(dt.Guid, false),
                        Guid = dt.Guid,
                        Owner_Guid = dt.Owner_Guid,
                        Title = dt.Title,
                        CommentCount = CommentClass.GetCommentCount(dt.Guid),
                        Seen = SeenClass.GetCount(dt.Guid)
                    };
                    //load colors
                    if (LikeClass.CheckExistence(dt.Guid, false, key))
                    {
                        loader.LikeColorString = "#1976D2";
                    }

                    if (DislikeClass.CheckExistence(dt.Guid, false, key))
                    {
                        loader.DislikeColorString = "#1976D2";
                    }

                    loaders.Add(loader);
                    loader = new ConfessLoader();
                }
                loaders = loaders.OrderByDescending(d => d.DateReal.Date).Reverse().ToList();
                return loaders;
            }

            private static ConfessLoader GetConfessLoader(Confess dt, string key)
            {
                ConfessLoader loader = new ConfessLoader
                {
                    Body = dt.Body,
                    Category = dt.Category,
                    Date = Shared.TimeAgo.Ago(dt.Date),//$"{dt.Date.ToLongDateString()} {dt.Date.ToShortTimeString()}",
                    DateReal = dt.Date,
                    DisLikes = DislikeClass.GetCount(dt.Guid, false),
                    Likes = LikeClass.GetCount(dt.Guid, false),
                    Guid = dt.Guid,
                    Owner_Guid = dt.Owner_Guid,
                    Title = dt.Title,
                    CommentCount = CommentClass.GetCommentCount(dt.Guid),
                    Seen = SeenClass.GetCount(dt.Guid)
                };
                //load colors
                if (LikeClass.CheckExistence(dt.Guid, false, key))
                {
                    loader.LikeColorString = "#1976D2";
                }

                if (DislikeClass.CheckExistence(dt.Guid, false, key))
                {
                    loader.DislikeColorString = "#1976D2";
                }

                return loader;
            }

        }

        public static class CommentClass
        {
            private static List<CommentLoader> GetLoader(List<Comment> list, string key)
            {
                List<CommentLoader> loaders = new List<CommentLoader>();
                foreach (Comment dt in list)
                {
                    CommentLoader loader = new CommentLoader
                    {
                        Body = dt.Body,
                        Date = Shared.TimeAgo.Ago(dt.Date),// $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(dt.Date.DayOfWeek)}, {CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dt.Date.Month)} {dt.Date.Day}, {dt.Date.Year} {dt.Date.ToShortTimeString()}",
                        DisLikes = DislikeClass.GetCount(dt.Guid, true),
                        DateReal = dt.Date,
                        Likes = LikeClass.GetCount(dt.Guid, true),
                        Guid = dt.Guid,
                        Owner_Guid = dt.Owner_Guid,
                    };
                    //load colors
                    if (LikeClass.CheckExistence(dt.Guid, true, key))
                    {
                        loader.LikeColorString = "#1976D2";
                    }

                    if (DislikeClass.CheckExistence(dt.Guid, true, key))
                    {
                        loader.DislikeColorString = "#1976D2";
                    }
                    if (dt.Owner_Guid == key)
                    {
                        loader.DeleteVisibility = true;
                    }

                    loaders.Add(loader);
                    loader = new CommentLoader();
                }
                loaders.OrderByDescending(d => d.DateReal);
                return loaders;
            }
            public static void DeleteComment(string guid)
            {
                contextLite.Comment.Delete(d => d.Guid == guid);

                //delete others
                contextLite.Likes.Delete(d => d.Comment_Guid == guid);
                contextLite.Dislikes.Delete(d => d.Comment_Guid == guid);

                //FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                //FilterDefinition<Comment> idFilter = builder.Eq(r => r.Guid, guid);
                //context.Comment.DeleteOne(idFilter);
            }

            public static void DeleteCommentWithConfessGuid(string confessguid)
            {
                contextLite.Comment.Delete(d => d.Confess_Guid == confessguid);

                //delete others
                contextLite.Likes.Delete(d=>d.Confess_Guid == confessguid);
                contextLite.Dislikes.Delete(d => d.Confess_Guid == confessguid);
                contextLite.Seen.Delete(d => d.Confess_Guid == confessguid);

                //FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                //FilterDefinition<Comment> idFilter = builder.Eq(r => r.Confess_Guid, confessguid);
                //context.Comment.DeleteMany(idFilter);
            }

            internal static string GetCommentCount(string guid)
            {
                int count = contextLite.Comment.Count(d => d.Confess_Guid == guid);
                return count.ToString();

                //FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                //FilterDefinition<Comment> idFilter = builder.Eq(r => r.Confess_Guid, guid);
                //long count = context.Comment.CountDocuments(idFilter);
                //return count.ToString();
            }
            public static ConfessLoader CreateComment(CommentPoster comment)
            {
                comment.Comment.Id = Logical.Setter(comment.Comment.Id);
                contextLite.Comment.Insert(comment.Comment);

                //context.Comment.InsertOne(comment.Comment);
                return ConfessClass.FetchOneConfessLoader(comment.ConfessGuid, comment.Key);
            }
            public static void UpdateComment(Comment comment)
            {
                Comment data = contextLite.Comment.FindOne(d => d.Guid == comment.Guid);
                comment.Id = data.Id;
                contextLite.Comment.Update(comment);

                //FilterDefinition<Comment> filter = Builders<Comment>.Filter.Eq(u => u.Guid, comment.Guid);
                //context.Comment.ReplaceOne(filter, comment);
            }
            public static List<CommentLoader> FetchComment(string guid, string key)
            {
                List<Comment> data = contextLite.Comment.Find(d => d.Confess_Guid == guid).ToList();

                //FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                //FilterDefinition<Comment> idFilter = builder.Eq(e => e.Confess_Guid, guid);
                //IFindFluent<Comment, Comment> cursor = context.Comment.Find(idFilter);
                // Find All

                //report seen
                Confess confess = ConfessClass.FetchOneConfessByGuid(guid);
                if (confess.Owner_Guid != key)
                {
                    SeenClass.Post(guid, key);
                }
                return GetLoader(data, key);
            }
        }

        public static class LikeClass
        {
            public static bool Post(string guid, bool isComment, string key)
            {
                //check if its disliked
                if (DislikeClass.CheckExistence(guid, isComment, key))
                {
                    return false;
                }
                //Check if there is already a like by the user
                if (CheckExistence(guid, isComment, key))
                {
                    //delete the like
                    Delete(guid, isComment, key);
                    return false;
                }
                //post new like
                Likes like = new Likes
                {
                    Owner_Guid = key,

                };
                if (isComment)
                {
                    like.Comment_Guid = guid;
                }
                else
                {
                    like.Confess_Guid = guid;
                }
                like.Id = Logical.Setter(like.Id);
                contextLite.Likes.Insert(like);
                //context.Likes.InsertOne(like);
                return true;
            }
            public static bool CheckExistence(string guid, bool isComment, string key)
            {
                int count = 0;
                if (isComment)
                {
                    count = contextLite.Likes.Count(d => d.Comment_Guid == guid & d.Owner_Guid == key);
                }
                else
                {
                    count = contextLite.Likes.Count(d => d.Confess_Guid == guid & d.Owner_Guid == key);
                }
                return count > 0;

                //FilterDefinitionBuilder<Likes> builder = Builders<Likes>.Filter;
                //FilterDefinition<Likes> idFilter;
                //if (isComment)
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //else
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //long cursor = context.Likes.CountDocuments(idFilter);
                //return cursor > 0;
            }
            public static void Delete(string guid, bool isComment, string key)
            {
                if (isComment)
                {
                    contextLite.Likes.Delete(d => d.Comment_Guid == guid & d.Owner_Guid == key);
                }
                else
                {
                    contextLite.Likes.Delete(d => d.Confess_Guid == guid & d.Owner_Guid == key);
                }

                //FilterDefinitionBuilder<Likes> builder = Builders<Likes>.Filter;
                //FilterDefinition<Likes> idFilter;
                //if (isComment)
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //else
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //context.Likes.DeleteMany(idFilter);
            }
            public static string GetCount(string guid, bool isComment)
            {
                int counter = 0;
                if (isComment)
                {
                    counter = contextLite.Likes.Count(d => d.Comment_Guid == guid);
                }
                else
                {
                    counter = contextLite.Likes.Count(d => d.Confess_Guid == guid);
                }
                return counter.ToString();

                //FilterDefinitionBuilder<Likes> builder = Builders<Likes>.Filter;
                //FilterDefinition<Likes> idFilter;
                //if (isComment)
                //{
                //    idFilter = builder.Eq(r => r.Comment_Guid, guid);
                //}
                //else
                //{
                //    idFilter = builder.Eq(r => r.Confess_Guid, guid);
                //}
                //long count = context.Likes.CountDocuments(idFilter);
                //return count.ToString();
            }
        }

        public static class DislikeClass
        {
            public static bool Post(string guid, bool isComment, string key)
            {

                //check if its liked
                if (LikeClass.CheckExistence(guid, isComment, key))
                {
                    return false;
                }

                //Check if there is already a Dislike by the user
                if (CheckExistence(guid, isComment, key))
                {
                    //delete the Dislike
                    Delete(guid, isComment, key);
                    return false;
                }

                //post new Dislike
                Dislikes Dislike = new Dislikes
                {
                    Owner_Guid = key,

                };
                if (isComment)
                {
                    Dislike.Comment_Guid = guid;
                }
                else
                {
                    Dislike.Confess_Guid = guid;
                }
                Dislike.Id = Logical.Setter(Dislike.Id);
                contextLite.Dislikes.Insert(Dislike);
                //.Dislikes.InsertOne(Dislike);
                return true;
            }
            public static bool CheckExistence(string guid, bool isComment, string key)
            {
                int count = 0;
                if (isComment)
                {
                    count = contextLite.Dislikes.Count(d => d.Comment_Guid == guid & d.Owner_Guid == key);
                }
                else
                {
                    count = contextLite.Dislikes.Count(d => d.Confess_Guid == guid & d.Owner_Guid == key);
                }
                return count > 0;

                //FilterDefinitionBuilder<Dislikes> builder = Builders<Dislikes>.Filter;
                //FilterDefinition<Dislikes> idFilter;
                //if (isComment)
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //else
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //long cursor = context.Dislikes.CountDocuments(idFilter);
                //return (cursor > 0);
            }
            public static void Delete(string guid, bool isComment, string key)
            {
                if (isComment)
                {
                    contextLite.Dislikes.Delete(d => d.Comment_Guid == guid & d.Owner_Guid == key);
                }
                else
                {
                    contextLite.Dislikes.Delete(d => d.Confess_Guid == guid & d.Owner_Guid == key);
                }
                //FilterDefinitionBuilder<Dislikes> builder = Builders<Dislikes>.Filter;
                //FilterDefinition<Dislikes> idFilter;
                //if (isComment)
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //else
                //{
                //    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //}
                //context.Dislikes.DeleteMany(idFilter);
            }
            public static string GetCount(string guid, bool isComment)
            {
                int counter = 0;
                if (isComment)
                {
                    counter = contextLite.Dislikes.Count(d => d.Comment_Guid == guid);
                }
                else
                {
                    counter = contextLite.Dislikes.Count(d => d.Confess_Guid == guid);
                }
                return counter.ToString();

                //FilterDefinitionBuilder<Dislikes> builder = Builders<Dislikes>.Filter;
                //FilterDefinition<Dislikes> idFilter;
                //if (isComment)
                //{
                //    idFilter = builder.Eq(r => r.Comment_Guid, guid);
                //}
                //else
                //{
                //    idFilter = builder.Eq(r => r.Confess_Guid, guid);
                //}
                //long count = context.Dislikes.CountDocuments(idFilter);
                //return count.ToString();
            }
        }

        public static class SeenClass
        {
            public static void Post(string guid, string key)
            {
                //Check if there is already a Seen by the user
                if (!CheckExistence(guid, key))
                {
                    //post new Seen
                    Seen Seen = new Seen
                    {
                        Owner_Guid = key,
                        Confess_Guid = guid

                    };
                    Seen.Id = Logical.Setter(Seen.Id);
                    contextLite.Seen.Insert(Seen);
                    // context.Seen.InsertOne(Seen);
                }
            }
            public static bool CheckExistence(string guid, string key)
            {
                int count = contextLite.Seen.Count(d => d.Confess_Guid == guid & d.Owner_Guid == key);
                return count > 0;
                //FilterDefinitionBuilder<Seen> builder = Builders<Seen>.Filter;
                //FilterDefinition<Seen> idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, key));
                //long curs = context.Seen.CountDocuments(idFilter);
                //return curs > 0;
            }
            public static string GetCount(string guid)
            {
                return contextLite.Seen.Count(d => d.Confess_Guid == guid).ToString();
                //FilterDefinitionBuilder<Seen> builder = Builders<Seen>.Filter;
                //FilterDefinition<Seen> idFilter = builder.Eq(r => r.Confess_Guid, guid);
                //long count = context.Seen.CountDocuments(idFilter);
                //return count.ToString();
            }
        }
    }
}
