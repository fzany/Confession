using System;

namespace Shared
{
    public static class TimeAgo
    {
        public static string Ago(this DateTime dateTime)
        {
            string result = string.Empty;
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    string.Format("{0} minutes ago", timeSpan.Minutes) :
                    "a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    string.Format("{0} hours ago", timeSpan.Hours) :
                    "an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    string.Format("{0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                int remainder, quotient = Math.DivRem(timeSpan.Days, 30, out remainder);
                if (quotient < 2)
                {
                    result = "a month ago";
                }
                else
                {
                    result = $"{remainder} months ago";
                }
            }
            else
            {
                int remainder, quotient = Math.DivRem(timeSpan.Days, 365, out remainder);
                if (quotient < 2)
                {
                    result = "a year ago";
                }
                else
                {
                    result = $"{remainder} years ago";
                }
            }

            return result;
        }
    }
}
