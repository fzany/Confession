using Confession.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Confession.Helpers
{
    public class Store
    {
        private static readonly DataContext context = new DataContext();

        public static class ConfessClass
        {
            public static async Task DeleteConfess(string guid)
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> idFilter = builder.Eq(r => r.Guid, guid);
                await context.Confess.DeleteOneAsync(idFilter);
            }
            public static async Task CreateConfess(Confess confess)
            {
                await context.Confess.InsertOneAsync(confess);
            }
            public static async Task UpdateConfess(Confess confess)
            {
                FilterDefinition<Confess> filter = Builders<Confess>.Filter.Eq(u => u.Guid, confess.Guid);
                await context.Confess.ReplaceOneAsync(filter, confess);
            }

            public static List<Confess> FetchConfessByCategory(string category)
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> idFilter = builder.Eq(e=>e.Category, category);

                IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find All
                return cursor.ToList();
            }

            public static async Task<List<Confess>> FetchMyConfessions()
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> idFilter = builder.Eq(e=>e.Owner_Guid, await Logic.GetKey());

                IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find All
                return cursor.ToList();
            }
            public static Confess FetchOneConfess(FieldDefinition<Confess, string> expression, string value)
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> idFilter = builder.Eq(expression, value);

                IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find One
                return cursor.FirstOrDefault();
            }

            public static Confess FetchOneConfessByGuid(string guid)
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> idFilter = builder.Eq(e=>e.Guid, guid);

                IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find One
                return cursor.FirstOrDefault();
            }

            public static Confess FetchOneConfess(FieldDefinition<Confess, int> expression, int value)
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> idFilter = builder.Eq(expression, value);

                IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);

                // Find One
                return cursor.FirstOrDefault();
            }

            public static List<Confess> FetchAllConfess()
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> empty = builder.Empty;
                IFindFluent<Confess, Confess> cursor = context.Confess.Find(empty).Limit(100);
                List<Confess> list = cursor.ToList();
                return list;
            }

            public static bool CheckExistence(FieldDefinition<Confess, string> expression, string value)
            {
                FilterDefinitionBuilder<Confess> builder = Builders<Confess>.Filter;
                FilterDefinition<Confess> idFilter = builder.Eq(expression, value);

                IFindFluent<Confess, Confess> cursor = context.Confess.Find(idFilter);
                // Find One
                return !(cursor.FirstOrDefault() == null);
            }
        }

        public static class CommentClass
        {
            public static void DeleteComment(string guid)
            {
                FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                FilterDefinition<Comment> idFilter = builder.Eq(r => r.Guid, guid);
                context.Comment.DeleteOne(idFilter);
            }

            internal static async Task<string> GetCommentCount(string guid)
            {
                FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                FilterDefinition<Comment> idFilter = builder.Eq(r => r.Confess_Guid, guid);
                long count = await context.Comment.CountDocumentsAsync(idFilter);
                return count.ToString();
            }
            public static void CreateComment(Comment comment)
            {
                context.Comment.InsertOne(comment);
            }
            public static void UpdateComment(Comment comment)
            {
                FilterDefinition<Comment> filter = Builders<Comment>.Filter.Eq(u => u.Guid, comment.Guid);
                context.Comment.ReplaceOne(filter, comment);
            }
            public static async Task<List<Comment>> FetchComment(string guid)
            {
                FilterDefinitionBuilder<Comment> builder = Builders<Comment>.Filter;
                FilterDefinition<Comment> idFilter = builder.Eq(e => e.Confess_Guid, guid);
                IAsyncCursor<Comment> cursor = await context.Comment.FindAsync(idFilter);
                // Find All
                return cursor.ToList();
            }
        }

        public static class LikeClass
        {
            public static async Task Post(string guid, bool isComment)
            {
                //check if its disliked
                if (await DislikeClass.CheckExistence(guid, isComment))
                {
                    return;
                }
                //Check if there is already a like by the user
                if (await CheckExistence(guid, isComment))
                {
                    //delete the like
                    await Delete(guid, isComment);
                }
                else
                {
                    //post new like
                    Likes like = new Likes
                    {
                        Owner_Guid = await Logic.GetKey(),

                    };
                    if (isComment)
                    {
                        like.Comment_Guid = guid;
                    }
                    else
                    {
                        like.Confess_Guid = guid;
                    }
                    context.Likes.InsertOne(like);
                }
            }
            public static async Task<bool> CheckExistence(string guid, bool isComment)
            {
                FilterDefinitionBuilder<Likes> builder = Builders<Likes>.Filter;
                FilterDefinition<Likes> idFilter;
                if (isComment)
                {
                    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                else
                {
                    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                IFindFluent<Likes, Likes> cursor = context.Likes.Find(idFilter);
                // Find One
                List<Likes> list = cursor.ToList();
                bool dat = (list.Count > 0);
                return dat;
            }
            public static async Task Delete(string guid, bool isComment)
            {
                FilterDefinitionBuilder<Likes> builder = Builders<Likes>.Filter;
                FilterDefinition<Likes> idFilter;
                if (isComment)
                {
                    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                else
                {
                    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                context.Likes.DeleteMany(idFilter);
            }
            public static async Task<string> GetCount(string guid, bool isComment)
            {
                FilterDefinitionBuilder<Likes> builder = Builders<Likes>.Filter;
                FilterDefinition<Likes> idFilter;
                if (isComment)
                {
                    idFilter = builder.Eq(r => r.Comment_Guid, guid);
                }
                else
                {
                    idFilter = builder.Eq(r => r.Confess_Guid, guid);
                }
                long count = await context.Likes.CountDocumentsAsync(idFilter);
                return count.ToString();
            }
        }

        public static class DislikeClass
        {
            public static async Task Post(string guid, bool isComment)
            {
                //check if its liked
                if (await LikeClass.CheckExistence(guid, isComment))
                {
                    return;
                }

                //Check if there is already a Dislike by the user
                if (await CheckExistence(guid, isComment))
                {
                    //delete the Dislike
                    await Delete(guid, isComment);
                }
                else
                {
                    //post new Dislike
                    Dislikes Dislike = new Dislikes
                    {
                        Owner_Guid = await Logic.GetKey(),

                    };
                    if (isComment)
                    {
                        Dislike.Comment_Guid = guid;
                    }
                    else
                    {
                        Dislike.Confess_Guid = guid;
                    }
                    context.Dislikes.InsertOne(Dislike);
                }
            }
            public static async Task<bool> CheckExistence(string guid, bool isComment)
            {
                FilterDefinitionBuilder<Dislikes> builder = Builders<Dislikes>.Filter;
                FilterDefinition<Dislikes> idFilter;
                if (isComment)
                {
                    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                else
                {
                    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                IFindFluent<Dislikes, Dislikes> cursor = context.Dislikes.Find(idFilter);
                // Find One
                List<Dislikes> list = cursor.ToList();
                return (list.Count > 0);
            }
            public static async Task Delete(string guid, bool isComment)
            {
                FilterDefinitionBuilder<Dislikes> builder = Builders<Dislikes>.Filter;
                FilterDefinition<Dislikes> idFilter;
                if (isComment)
                {
                    idFilter = builder.And(builder.Eq(f => f.Comment_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                else
                {
                    idFilter = builder.And(builder.Eq(f => f.Confess_Guid, guid), builder.Eq(v => v.Owner_Guid, await Logic.GetKey()));
                }
                context.Dislikes.DeleteMany(idFilter);
            }
            public static async Task<string> GetCount(string guid, bool isComment)
            {
                FilterDefinitionBuilder<Dislikes> builder = Builders<Dislikes>.Filter;
                FilterDefinition<Dislikes> idFilter;
                if (isComment)
                {
                    idFilter = builder.Eq(r => r.Comment_Guid, guid);
                }
                else
                {
                    idFilter = builder.Eq(r => r.Confess_Guid, guid);
                }
                long count = await context.Dislikes.CountDocumentsAsync(idFilter);
                return count.ToString();
            }
        }

    }
}
