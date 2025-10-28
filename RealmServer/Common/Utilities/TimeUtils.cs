#region

using System;
using System.Globalization;
using System.Text.RegularExpressions;

#endregion

namespace Common.Utilities
{
    public static class TimeUtils
    {
        public static int ToUnixTimestamp(this DateTime dateTime)
        {
            return (int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime FromUnixTimestamp(int time)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(time).ToLocalTime();
            return dateTime;
        }

        public static TimeSpan? ParseTimeSpan(string input)
        {
            var regex = new Regex(@"(\d+)([smhdw])", RegexOptions.IgnoreCase);
            var matches = regex.Matches(input);
            if (matches.Count == 0) return null;

            var timeSpan = TimeSpan.Zero;
            foreach (Match match in matches)
            {
                var value = int.Parse(match.Groups[1].Value);
                var unit = char.ToLower(match.Groups[2].Value[0]);

                switch (unit)
                {
                    case 's':
                        timeSpan = timeSpan.Add(TimeSpan.FromSeconds(value));
                        break;
                    case 'm':
                        timeSpan = timeSpan.Add(TimeSpan.FromMinutes(value));
                        break;
                    case 'h':
                        timeSpan = timeSpan.Add(TimeSpan.FromHours(value));
                        break;
                    case 'd':
                        timeSpan = timeSpan.Add(TimeSpan.FromDays(value));
                        break;
                    case 'w':
                        timeSpan = timeSpan.Add(TimeSpan.FromDays(value * 7));
                        break;
                }
            }

            return timeSpan;
        }

        private static readonly string[] dateFormats = new[] { "MM/dd/yyyy hh:mmtt", "MM/dd hh:mmtt", "dd hh:mmtt", "MM/dd/yyyy h:mmtt", "MM/dd h:mmtt", "dd h:mmtt", "MM/dd/yyyy hhtt", "MM/dd hhtt", "dd hhtt" };
        private static readonly string[] timeFormats = new[] { "hh:mmtt", "h:mmtt", "hhtt", "htt" };

        public static DateTime? ParseDateTime(string input)
        {
            var now = DateTime.UtcNow;
            DateTime result;

            // Try parsing full date and time with minutes
            if (DateTime.TryParseExact(input, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                // If year or month is missing, use current year and month
                if (result.Year == 1) result = result.AddYears(now.Year - 1);
                if (result.Month == 1) result = result.AddMonths(now.Month - 1);
                return result.ToUniversalTime();
            }

            // Try parsing time only
            if (DateTime.TryParseExact(input, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                result = new DateTime(now.Year, now.Month, now.Day, result.Hour, result.Minute, 0);
                return result.ToUniversalTime();
            }

            return null;
        }

        // https://www.codeproject.com/Articles/770323/How-to-Convert-a-Date-Time-to-X-minutes-ago-in-Csh
        public static string TimeAgo(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                var years = span.Days / 365;
                if (span.Days % 365 != 0)
                    years += 1;
                return $"{years} {(years == 1 ? "year" : "years")} ago";
            }

            if (span.Days > 30)
            {
                var months = span.Days / 30;
                if (span.Days % 31 != 0)
                    months += 1;
                return $"{months} {(months == 1 ? "month" : "months")} ago";
            }

            if (span.Days > 0)
                return $"{span.Days} {(span.Days == 1 ? "day" : "days")} ago";
            if (span.Hours > 0)
                return $"{span.Hours} {(span.Hours == 1 ? "hour" : "hours")} ago";
            if (span.Minutes > 0)
                return $"{span.Minutes} {(span.Minutes == 1 ? "minute" : "minutes")} ago";
            if (span.Seconds > 5)
                return $"{span.Seconds} seconds ago";
            if (span.Seconds <= 5)
                return "just now";
            return string.Empty;
        }
    }
}