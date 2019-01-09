using Confession.Models;
using MongoDB.Driver;

namespace Confession.Helpers
{
    public class DataContext
    {
        public IMongoDatabase Database;
        public DataContext()
        {
            MongoClient client = new MongoClient(@"mongodb://devtest:ws8CGV6bEc2WLDT@azurecluster-shard-00-00-j6ddx.azure.mongodb.net:27017,azurecluster-shard-00-01-j6ddx.azure.mongodb.net:27017,azurecluster-shard-00-02-j6ddx.azure.mongodb.net:27017/test?ssl=true&replicaSet=AzureCluster-shard-0&authSource=admin&retryWrites=true");
            Database = client.GetDatabase("confession");

        }
        public IMongoCollection<Confess> Confess => Database.GetCollection<Confess>(typeof(Confess).Name.ToLower());
        public IMongoCollection<User> User => Database.GetCollection<User>(typeof(User).Name.ToLower());
        public IMongoCollection<Comment> Comment => Database.GetCollection<Comment>(typeof(Comment).Name.ToLower());

        public IMongoCollection<Likes> Likes => Database.GetCollection<Likes>(typeof(Likes).Name.ToLower());
        public IMongoCollection<Dislikes> Dislikes => Database.GetCollection<Dislikes>(typeof(Dislikes).Name.ToLower());

    }


}
