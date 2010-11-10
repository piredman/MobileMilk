using System;

namespace MobileMilk.Common.Extensions
{
    public static class StringExtentions
    {
        public static int AsInt(this string value, int defaultValue)
        {
            int returnValue;
            if (!int.TryParse(value, out returnValue))
                returnValue = defaultValue;

            return returnValue;
        }

        public static decimal AsDecimal(this string value, decimal defaultValue)
        {
            decimal returnValue;
            if (!decimal.TryParse(value, out returnValue))
                returnValue = defaultValue;

            return returnValue;
        }

        public static bool AsBool(this string value, bool defaultValue)
        {
            bool returnValue;
            if (!bool.TryParse(value, out returnValue))
                returnValue = (value == "1");

            return returnValue;
        }

        public static DateTime AsDateTime(this string value, DateTime defaultValue)
        {
            DateTime returnValue;
            if (!DateTime.TryParse(value, out returnValue))
                returnValue = defaultValue;

            return returnValue;
        }

        public static DateTime? AsNullableDateTime(this string value, DateTime? defaultValue)
        {
            DateTime workingValue;
            DateTime? returnValue;
            returnValue = DateTime.TryParse(value, out workingValue) ? workingValue : defaultValue;

            return returnValue;
        }
    }
}
