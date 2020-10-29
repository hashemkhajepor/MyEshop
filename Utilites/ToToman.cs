using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon
{
    public static class ToToman
    {
        public static string Toman(this int value)
        {
            return value.ToString("#,0") + " تومان";
        }
    }
}