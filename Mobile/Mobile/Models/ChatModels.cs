using Mobile.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class ChatRoomLoader
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } 
        public string Title { get; set; }
        public string MembersCountLogo { get; set; }= Constants.FontAwe.Users;
        public string MembersCount { get; set; } = 0.ToString();
        public string ChatsCountLogo { get; set; } = Constants.FontAwe.Comments;
        public string ChatsCount { get; set; } = 0.ToString();
        public bool ISUnread { get; set; }
        public bool IamMember { get; set; }
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

        //image
        public string ImageUrl { get; set; }
        public bool IsImageAvailable { get; set; }
    }

    public class ChatLoader
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string ChatId { get; set; }
        public string Body { get; set; }
        public string SenderName { get; set; }
        public string SenderKey { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Time => Logic.GetTimeFromDate(Date);
        public bool IsMine { get; set; }
        public string Room_ID { get; set; } = string.Empty;
        public bool IsAd { get; set; }
        public Color SenderColor => Logic.GetColourFromName(SenderName);

        //quotedChat
        public bool QuotedChatAvailable { get; set; }
        public Quote Quote { get; set; }

        public bool IsSent { get; set; }

        //image
        public string ImageUrl { get; set; }
        public bool IsImageAvailable { get; set; }

        //internalImage
        public ImageSource ImageSource { get; set; }

    }

    public class Quote
    {
        public string Body { get; set; }
        public string OwnerName { get; set; }
        public string OwnerKey { get; set; }
        public Color SenderColor => Logic.GetColourFromName(OwnerName);
        public string SenderNameShow { get; set; }

        //image
        public string ImageUrl { get; set; }
        public bool IsImageAvailable { get; set; }

        //internalImage
        public ImageSource ImageSource { get; set; }

    }

    public class ImageSender
    {
        public Stream stream { get; set; }
        public string body { get; set; }
    }
}
