using LiteDB;
using Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Mobile.Helpers.Local
{
    public class LocalDataContext
    {
        public LiteDatabase Database;

        public LocalDataContext()
        {
            Database = new LiteDatabase(DependencyService.Get<IDataBaseAccess>().DatabasePath());
        }
        public LiteCollection<ConfessLoader> ConfessLoader => Database.GetCollection<ConfessLoader>(typeof(ConfessLoader).Name.ToLower());
        public LiteCollection<CommentLoader> CommentLoader => Database.GetCollection<CommentLoader>(typeof(CommentLoader).Name.ToLower());
        public LiteCollection<ChatLoader> ChatLoader => Database.GetCollection<ChatLoader>(typeof(ChatLoader).Name.ToLower());
        public LiteCollection<ChatRoomLoader> ChatRoomLoader => Database.GetCollection<ChatRoomLoader>(typeof(ChatRoomLoader).Name.ToLower());


        public LiteCollection<BsonDocument> ConfessRaw => Database.GetCollection<BsonDocument>(typeof(ConfessLoader).Name.ToLower());
        public LiteCollection<BsonDocument> CommentRaw => Database.GetCollection<BsonDocument>(typeof(CommentLoader).Name.ToLower());
        public LiteCollection<BsonDocument> ChatRaw => Database.GetCollection<BsonDocument>(typeof(ChatLoader).Name.ToLower());
        public LiteCollection<BsonDocument> ChatRoomRaw => Database.GetCollection<BsonDocument>(typeof(ChatRoomLoader).Name.ToLower());

    }
}
