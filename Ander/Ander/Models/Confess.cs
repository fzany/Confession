using Ander.Helpers;

using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Ander.Models
{
    public class Confess
    {
        
        
        public string Id { get; set; }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString().Replace("-","");
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Owner_Guid { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class ConfessLoader
    {
        public string Guid { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Likes { get; set; } = string.Empty;
        public string DisLikes { get; set; } = string.Empty;
        public string LikesLogo { get; set; } = Constants.FontAwe.Thumbs_up;
        public string DisLikesLogo { get; set; } = Constants.FontAwe.Thumbs_down;
        public string Date { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string CommentCount { get; set; } = string.Empty;
        public string CommentLogo { get; set; } = Constants.FontAwe.Comments;

        public Color LikeColor { get; set; }
        public Color DislikeColor { get; set; }

        public string LikeColorString { get; set; } = string.Empty;
        public string DislikeColorString { get; set; } = string.Empty;

        public string Seen { get; set; } = string.Empty;
        public string SeenLogo { get; set; } = Constants.FontAwe.Eye;
    }
    public class ConfessSender
    {
        public ConfessLoader Loader { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
