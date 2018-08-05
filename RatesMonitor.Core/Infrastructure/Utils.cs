using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RatesMonitor.Core.Infrastructure
{
    public static class Utils
    {
        public static decimal DecimalParse(this string str)
        {
            str = str.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            str = str.Replace(".", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            return decimal.Parse(str, CultureInfo.InvariantCulture);
        }

        public static int EuroDayOfWeek(this DateTime dt)
        {
            return dt.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)dt.DayOfWeek;
        }
    }
}
