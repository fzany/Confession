using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Logg
    {
        public string Body { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Method { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
    }
}
