using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using LiteDB;
using Microsoft.Extensions.Hosting.Internal;
using Shared;

namespace Backend.Helpers
{
    public class DataContextLite
    {
        public LiteDatabase Database;
        public DataContextLite()
        {

            FileStream file = File.Open("MyData.db", System.IO.FileMode.OpenOrCreate);
            string dbPath = file.Name;
            file.Dispose();
            Database = new LiteDatabase(dbPath);
        }
        public LiteCollection<Confess> Confess => Database.GetCollection<Confess>(typeof(Confess).Name.ToLower());
        public LiteCollection<User> User => Database.GetCollection<User>(typeof(User).Name.ToLower());
        public LiteCollection<Comment> Comment => Database.GetCollection<Comment>(typeof(Comment).Name.ToLower());
        public LiteCollection<Likes> Likes => Database.GetCollection<Likes>(typeof(Likes).Name.ToLower());
        public LiteCollection<Dislikes> Dislikes => Database.GetCollection<Dislikes>(typeof(Dislikes).Name.ToLower());
        public LiteCollection<Seen> Seen => Database.GetCollection<Seen>(typeof(Seen).Name.ToLower());
        public LiteCollection<Shared.Logger> Logger => Database.GetCollection<Shared.Logger>(typeof(Shared.Logger).Name.ToLower());

    }
}
