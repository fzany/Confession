using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Shared
{
    public class ChatRoomLoader
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string MembersCount { get; set; } = 0.ToString();
        public bool IamMember { get; set; }
        public string ChatsCount { get; set; } = 0.ToString();
    }

    public class ChatRoom
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string Title { get; set; }
        public List<string> Members { get; set; }
    }

    public class Chat
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string SenderKey { get; set; }
        public string Body { get; set; }
        public string SenderName { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Room_ID { get; set; } = string.Empty;

        //quotedChat
        public bool QuotedChatAvailable { get; set; }
        public Quote Quote { get; set; }
    }

    public class ChatLoader
    {
        public string Body { get; set; }
        public string SenderName { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsMine { get; set; }
        public string Room_ID { get; set; } = string.Empty;
        public string ChatId { get; set; }

        //quotedChat
        public bool QuotedChatAvailable { get; set; }
        public Quote Quote { get; set; }
    }
    public class Quote
    {
        public string Body { get; set; }
        public string SenderName { get; set; }
        public string SenderKey { get; set; }
    }
}
