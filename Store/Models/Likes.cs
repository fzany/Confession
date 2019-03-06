using System;

namespace Uwp.Models
{
    public class Likes
    {

        public string Id { get; set; }
        public string Guid { get; set; } = Store.Helpers.ObjectIds.GenerateNewId().ToString();
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
        public string Comment_Guid { get; set; } = string.Empty;

    }

    public class Dislikes
    {
        
        public string Id { get; set; }
        public string Guid { get; set; } = Store.Helpers.ObjectIds.GenerateNewId().ToString();
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
        public string Comment_Guid { get; set; } = string.Empty;
    }
}
