using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Amazon
{
    public static class DateConvert
    {
        public static string ToShamsi(this DateTime value)
        {
            PersianCalendar PC = new PersianCalendar();
            return PC.GetYear(value) + "/" + PC.GetMonth(value).ToString("00") + "/" + PC.GetDayOfMonth(value).ToString("00");
        }
    }
}