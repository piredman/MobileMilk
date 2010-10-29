using System;

namespace MobileMilk.Common
{
    public static class Convert
    {
        public static DateTime? AsDateTime(string value)
        {
            if (null == value)
                return null;

            DateTime date;
            if (!DateTime.TryParse(value, out date))
                date = DateTime.MinValue;

            return (DateTime.MinValue != date) ? (DateTime?) date : null;
        }

        public static bool AsBoolean(string value)
        {
            if (null == value)
                return false;

            bool boolean;
            if (!Boolean.TryParse(value, out boolean))
                boolean = false;

            return boolean;
        }

        public static int AsInt(string value)
        {
            if (null == value)
                return 0;

            int integer;
            if (!int.TryParse(value, out integer))
                integer = 0;

            return integer;
        }
    }
}
