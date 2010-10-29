using System;

namespace MobileMilk.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AsDateTime(this DateTime? value)
        {
            return (value ?? DateTime.MinValue);
        }

        public static DateTime AsDateTime(this DateTime? value, DateTime defaultValue)
        {
            return (value ?? defaultValue);
        }

        public static DateTime StartOfWeek(this DateTime value, DayOfWeek startOfWeek)
        {
            var diff = value.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;

            return value.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime value, DayOfWeek startOfWeek)
        {
            var diff = value.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;

            return value.AddDays(diff).Date;
        }
    }

}
