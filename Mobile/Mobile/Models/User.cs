using Mobile.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Models
{
    public class UserData
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string AppCenterID { get; set; }
        public bool ChatRoomNotification { get; set; }
        public bool CommentNotification { get; set; }
        public bool Biometry { get; set; }
        public List<string> Key { get; set; }
        public DeviceInfo Logger { get; set; }
    }
}
