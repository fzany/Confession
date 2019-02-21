
using System;
using System.Collections.Generic;
using System.Text;

namespace Ander.Models
{
    public class User
    {
        
        
        public string Id { get; set; }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString().Replace("-", "");
        public string Name { get; set; } = string.Empty;
    }
}
