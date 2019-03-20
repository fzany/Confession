using Store.Helpers;
using System;
using Windows.UI;

namespace Uwp.Models
{
    public class Confess
    {
        public string Id { get; set; }
        public string Guid { get; set; } = Store.Helpers.ObjectIds.GenerateNewId().ToString();
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

        public Windows.UI.Xaml.Media.Brush LikeColor { get; set; }
        public Windows.UI.Xaml.Media.Brush DislikeColor { get; set; }

        public Windows.UI.Xaml.Visibility ShowAds { get; set; } = Windows.UI.Xaml.Visibility.Collapsed;

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
