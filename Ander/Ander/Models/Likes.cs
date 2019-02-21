
using System;
using System.Collections.Generic;
using System.Text;

namespace Ander.Models
{
    public class Likes
    {
        
        
        public string Id { get; set; }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString().Replace("-", "");
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
        public string Comment_Guid { get; set; } = string.Empty;

    }

    public class Dislikes
    {
        
        
        public string Id { get; set; }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString().Replace("-", "");
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
        public string Comment_Guid { get; set; } = string.Empty;
    }
}
