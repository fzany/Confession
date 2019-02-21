using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = System.Guid.NewGuid().ToString().Replace("-", "");
        public string Guid { get; set; } = System.Guid.NewGuid().ToString().Replace("-", "");
        public string Name { get; set; } = string.Empty;
    }
}
