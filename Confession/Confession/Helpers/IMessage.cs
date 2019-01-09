using System;
using System.Collections.Generic;
using System.Text;

namespace Confession.Helpers
{
    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
