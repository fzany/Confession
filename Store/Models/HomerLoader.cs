using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class HomerLoader
    {
        public LoadMode loadMode { get; set; } = LoadMode.None;
        public string Category { get; set; } = string.Empty;
    }
}
