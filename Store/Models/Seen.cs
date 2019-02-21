using System;

namespace Store.Models
{
    public class Seen
    {
        public string Id { get; set; }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString().Replace("-", "");
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Confess_Guid { get; set; } = string.Empty;
        public string Owner_Guid { get; set; } = string.Empty;
    }
}
