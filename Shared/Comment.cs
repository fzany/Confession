using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Shared
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Guid { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Body { get; set; } = string.Empty;
        [BsonDateTimeOptions(Kind =DateTimeKind.Utc)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;

        public CommentQuote Quote { get; set; }
        public bool QuotedCommentAvailable { get; set; }
    }
    public class CommentLoader
    {
        public string Guid { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Likes { get; set; } = string.Empty;
        public string DisLikes { get; set; } = string.Empty;
        public string LikesLogo { get; set; } = Constants.FontAwe.Thumbs_up;
        public string DisLikesLogo { get; set; } = Constants.FontAwe.Thumbs_down;
        public string Date { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
        public DateTime DateReal { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;

        public string LikeColorString { get; set; } = string.Empty;
        public string DislikeColorString { get; set; } = string.Empty;

        public string DeleteLogo { get; set; } = Constants.FontAwe.Trash;
        public bool DeleteVisibility { get; set; } = false;

        public CommentQuote Quote { get; set; }
        public bool QuotedCommentAvailable { get; set; }
    }

    public class CommentQuote
    {
        public string Body { get; set; }
    }
    public class CommentPoster
    {
        public Comment Comment { get; set; }
        public string Key { get; set; } = string.Empty;
        public string ConfessGuid { get; set; } = string.Empty;
    }
    public class CommentSender
    {
        public string CommentGuid { get; set; }
        public string Key { get; set; } = string.Empty;
        public bool IsComment { get; set; }
        public string ConfessGuid { get; set; } = string.Empty;
        public string Count { get; set; }

    }
}
