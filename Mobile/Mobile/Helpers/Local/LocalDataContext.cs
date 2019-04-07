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

        public LiteCollection<UserData> UserData => Database.GetCollection<UserData>(typeof(UserData).Name.ToLower());
        public LiteCollection<Likes> Likes => Database.GetCollection<Likes>(typeof(Likes).Name.ToLower());
        public LiteCollection<Dislikes> Dislikes => Database.GetCollection<Dislikes>(typeof(Dislikes).Name.ToLower());
        public LiteCollection<Seen> Seen => Database.GetCollection<Seen>(typeof(Seen).Name.ToLower());
        public LiteCollection<ChatRoom> ChatRoom => Database.GetCollection<ChatRoom>(typeof(ChatRoom).Name.ToLower());

    }
}
