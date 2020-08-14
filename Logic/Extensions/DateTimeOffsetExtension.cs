using System;

namespace Logic.Extensions
{
    public static class DateTimeOffsetExtension
    {
        public static string ToDisplayDate(this DateTimeOffset date)
        {
            var day = date.Day;
            var dayStringOrdinal = day +
                                   (day % 10 == 1 && day != 11 ? "st"
                                       : day % 10 == 2 && day != 12 ? "nd"
                                       : day % 10 == 3 && day != 13 ? "rd" : "th");

            return date.ToString("MMMM DAY, h:mm tt").Replace("DAY", dayStringOrdinal);
        }
    }
}