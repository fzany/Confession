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
                return MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            return id;
        }
    }
}
