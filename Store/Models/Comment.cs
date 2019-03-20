using System;
using Windows.UI;

namespace Store.Models
{
    public class Comment
    {
      
        public string Id { get; set; }
        public string Guid { get; set; } = Store.Helpers.ObjectIds.GenerateNewId().ToString();
        public string Body { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
    }
    public class CommentLoader
    {
        public string Guid { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Likes { get; set; } = string.Empty;
        public string DisLikes { get; set; } = string.Empty;
        public string LikesLogo { get; set; } //= Constants.FontAwe.Thumbs_up;
        public string DisLikesLogo { get; set; } //= Constants.FontAwe.Thumbs_down;
        public string Date { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;

        public Windows.UI.Xaml.Media.Brush LikeColor { get; set; }
        public Windows.UI.Xaml.Media.Brush DislikeColor { get; set; }

        public string LikeColorString { get; set; } = string.Empty;
        public string DislikeColorString { get; set; } = string.Empty;

        public string DeleteLogo { get; set; } //= Constants.FontAwe.Trash;
        public bool DeleteVisibility { get; set; } = false;

    }
    public enum LoadMode
    {
        Mine, Category, None
    }
    public class CommentPoster
    {
        public Comment Comment { get; set; }
        public string Key { get; set; } = string.Empty;
        public string ConfessGuid { get; set; } = string.Empty;
    }
}
