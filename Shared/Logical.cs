using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Logical
    {
        public static string Setter(string id)
        {
            if(string.IsNullOrEmpty(id))
                return System.Guid.NewGuid().ToString().Replace("-", "");
            return id;
        }
    }
}
