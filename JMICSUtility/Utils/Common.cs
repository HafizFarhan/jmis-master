
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MTC.JMICS.Utility.Utils
{
    public class Common
    {
        public static string WeatherAPIKey { get; } = "55b7af552a5e0d6552e646517031354c";

        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
        public static string TrimEncryptedString(string value)
        {
            while (value.EndsWith("="))
                value = value.Substring(0, value.Length - 1);

            return value;
        }

        public static string FixLengthOfEncryptedString(string value)
        {
            if (value.Length % 4 > 0)
                value = value.PadRight(value.Length + 4 - value.Length % 4, '=');

            return value;
        }

        /// <summary>
        ///  Compare two objects using reflection , compare values of properties and return true / false
        /// </summary>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        /// <returns></returns>
        public static bool AreObjectsEqual(object objectA, object objectB)
        {
            bool result;
            result = true;


            if (objectA != null && objectB != null)
            {

                Type objectType;
                objectType = objectA.GetType();
                result = true; // assume by default they are equal


                foreach (PropertyInfo propertyInfo in objectType.GetProperties())
                //BindingFlags.Public | BindingFlags.Instance).Where(
                //p => p.CanRead && !ignoreList.Contains(p.Name)))
                {
                    object valueA;
                    object valueB;

                    valueA = propertyInfo.GetValue(objectA, null);
                    valueB = propertyInfo.GetValue(objectB, null);

                    if (propertyInfo.Name == "LastModifiedBy" || propertyInfo.Name == "LastModifiedOn")
                    {
                        // do nothing
                    }
                    else
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Compate two Object tyoe Comparing there values and return true / false
        /// </summary>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        /// <returns></returns>
        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            // commented out because update was failing. For Example, Client Mailing Zip Code "75290" and ""
            if ((!string.IsNullOrEmpty(Convert.ToString(valueA)) && string.IsNullOrEmpty(Convert.ToString(valueB)))
                || (string.IsNullOrEmpty(Convert.ToString(valueA)) && !string.IsNullOrEmpty(Convert.ToString(valueB))))
                result = false; // one of the values is null
            else if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }

        public static DateTime GetLocalDateTime(string timeZone)
        {
            if (string.IsNullOrEmpty(timeZone))
                return DateTime.Now;
            else
            {
                DateTime utc = DateTime.UtcNow;
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, zone);
                return localDateTime;
            }
        }

        public static DateTime GetLocalDateTime(string timeZone, DateTime dateTime)
        {
            try
            {
                DateTime utc = dateTime.ToUniversalTime();
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, zone);
                return localDateTime;
            }
            catch
            {
                return DateTime.Now;
            }
        }


        public static void TimePeriod(TimePeriodName timePeriod, out DateTime? startDate, out DateTime? endDate)
        {
            switch (timePeriod)
            {

                case (TimePeriodName.TODAY):
                    {
                        startDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString());
                        endDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString());
                    }
                    break;
                case (TimePeriodName.THISWEEK):
                    {
                        startDate = Convert.ToDateTime(getLastSunday(DateTime.UtcNow).ToShortDateString());
                        endDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString());
                    }
                    break;
                case (TimePeriodName.LASTWEEK):
                    {
                        startDate = Convert.ToDateTime(getLastSunday(DateTime.UtcNow).AddDays(-7).ToShortDateString());
                        endDate = Convert.ToDateTime(getLastSunday(DateTime.UtcNow).AddDays(-1).ToShortDateString());
                    }
                    break;
                case (TimePeriodName.THISMONTH):
                    {
                        startDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.Month + "/" + 1 + "/" + DateTime.UtcNow.Year).ToShortDateString());
                        endDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString());
                    }
                    break;
                case (TimePeriodName.LASTMONTH):
                    {
                        startDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.AddMonths(-1).Month + "/" + 1 + "/" + DateTime.UtcNow.Year).ToShortDateString());
                        endDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.Month + "/" + 1 + "/" + DateTime.UtcNow.Year).AddDays(-1).ToShortDateString());
                    }
                    break;
                case (TimePeriodName.THISYEAR):
                    {
                        startDate = Convert.ToDateTime(DateTime.Parse(1 + "/" + 1 + "/" + DateTime.UtcNow.Year).ToShortDateString());
                        endDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString());
                    }
                    break;
                case (TimePeriodName.LASTYEAR):
                    {
                        startDate = Convert.ToDateTime(DateTime.Parse(1 + "/" + 1 + "/" + DateTime.UtcNow.AddYears(-1).Year).ToShortDateString());
                        endDate = Convert.ToDateTime(DateTime.Parse(1 + "/" + 1 + "/" + DateTime.UtcNow.Year).AddDays(-1).ToShortDateString());
                    }
                    break;
                default:
                    startDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString());
                    endDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString());
                    break;
            }
        }

        public static void TimePeriod(TimePeriodName timePeriod, DateTime subscriberDateTimeNow, out DateTime? startDate, out DateTime? endDate)
        {
            switch (timePeriod)
            {


                case (TimePeriodName.TODAY):
                    {
                        startDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 0, 0, 0);
                        endDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 23, 59, 59);
                    }
                    break;

                case (TimePeriodName.YESTERDAY):
                    {
                        DateTime yesterday = subscriberDateTimeNow.AddDays(-1);

                        startDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0);
                        endDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                    }
                    break;

                case (TimePeriodName.THISWEEK):
                    {
                        DateTime lastSunday = getLastSunday(subscriberDateTimeNow);

                        startDate = new DateTime(lastSunday.Year, lastSunday.Month, lastSunday.Day, 0, 0, 0);
                        endDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 23, 59, 59);
                    }
                    break;
                case (TimePeriodName.LASTWEEK):
                    {
                        DateTime prevWeekSunday = getLastSunday(subscriberDateTimeNow).AddDays(-7);
                        DateTime lastSunday = getLastSunday(subscriberDateTimeNow).AddDays(-1);

                        startDate = new DateTime(prevWeekSunday.Year, prevWeekSunday.Month, prevWeekSunday.Day, 0, 0, 0);
                        endDate = new DateTime(lastSunday.Year, lastSunday.Month, lastSunday.Day, 23, 59, 59);
                    }
                    break;
                case (TimePeriodName.THISMONTH):
                    {
                        startDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, 1, 0, 0, 0);
                        endDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 23, 59, 59);
                    }
                    break;
                case (TimePeriodName.LASTMONTH):
                    {
                        startDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, 1, 0, 0, 0).AddMonths(-1);
                        endDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, 1, 23, 59, 59).AddDays(-1);
                    }
                    break;
                case (TimePeriodName.THISYEAR):
                    {
                        startDate = new DateTime(subscriberDateTimeNow.Year, 1, 1, 0, 0, 0);
                        endDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 23, 59, 59);
                    }
                    break;
                case (TimePeriodName.LASTYEAR):
                    {
                        startDate = new DateTime(subscriberDateTimeNow.Year, 1, 1, 0, 0, 0).AddYears(-1);
                        endDate = new DateTime(subscriberDateTimeNow.Year, 1, 1, 23, 59, 59).AddDays(-1);
                    }
                    break;
                case (TimePeriodName.DAYS90):
                    {
                        startDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, 1, 0, 0, 0).AddMonths(-1);
                        endDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 23, 59, 59);
                    }
                    break;
                default:
                    startDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 23, 59, 59).AddDays(-90);
                    endDate = new DateTime(subscriberDateTimeNow.Year, subscriberDateTimeNow.Month, subscriberDateTimeNow.Day, 23, 59, 59);
                    break;
            }
        }

        private static DateTime getLastSunday(DateTime aDate)
        {
            return aDate.AddDays(7 - (int)aDate.DayOfWeek).AddDays(-7);
        }


        public static string GetBase64Content(string fileURL)
        {
            try
            {
                return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileURL));
            }
            catch { return ""; }

        }

        //public static string GetTimeZoneIDFromAbbreviation(string abbreviation)
        //{
        //    string zoneID = null;
        //    var inclusions = Constants.TimeZone.Inclusions;

        //    foreach (var tz in TimeZoneInfo.GetSystemTimeZones()
        //                  .Where(zone => inclusions.Any(x => zone.Id.Contains(x))))
        //    {

        //        zoneID = tz.Id;
        //        string lang = CultureInfo.CurrentCulture.Name;   // example: "en-US"
        //        var zoneAbbr = TZNames.GetAbbreviationsForTimeZone(zoneID, lang);
        //        //var zoneAbbr = zoneID.CapitalLetters();
        //        if (zoneAbbr.Standard == abbreviation)
        //            break;

        //    }
        //    return zoneID;
        //}

        //public static string GetTimeZoneAbbreviationFromID(string timeZoneID)
        //{
        //    string tzid = timeZoneID;                // example: "Eastern Standard time"
        //    string lang = CultureInfo.CurrentCulture.Name;   // example: "en-US"
        //    var zoneAbbr = TZNames.GetAbbreviationsForTimeZone(tzid, lang);
        //    return zoneAbbr.Standard;

        //}

        //public static string GetTzAbbreviation(string timeZoneName)
        //{
        //    string output = string.Empty;

        //    if (timeZoneName != "UTC")
        //    {

        //        string[] timeZoneWords = timeZoneName.Split(' ');
        //        foreach (string timeZoneWord in timeZoneWords)
        //        {
        //            if (timeZoneWord[0] != '(')
        //            {
        //                output += timeZoneWord[0];
        //            }
        //            else
        //            {
        //                output += timeZoneWord;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        output = "UTC";
        //    }
        //    return output;
        //}

        //public static String SubscriberTimeZoneAbbr(int subscriberID = 0)
        //{
        //    MemCache memCache = new MemCache();
        //    string timeZoneCachekey = memCache.CreateCacheKey("timezone" + subscriberID);
        //    if (memCache.GetValue(timeZoneCachekey) != null && memCache.GetValue(timeZoneCachekey) != "")
        //    {
        //        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(memCache.GetValue(timeZoneCachekey).ToString());
        //        //var zoneName = zone.Id;/* zone.IsDaylightSavingTime(DateTime.UtcNow)
        //        //    ? zone.DaylightName
        //        //    : zone.StandardName;*/
        //        //var zoneAbbr = zoneName.CapitalLetters();
        //        string tzid = zone.Id;                // example: "Eastern Standard time"
        //        string lang = CultureInfo.CurrentCulture.Name;   // example: "en-US"
        //        var zoneAbbr = TZNames.GetAbbreviationsForTimeZone(tzid, lang);
        //        return zoneAbbr.Standard;
        //    }
        //    else
        //    {
        //        return "UTC";
        //    }
        //}


        public static Uri GetAbsoluteUri(HttpRequest request)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            uriBuilder.Path = request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();
            return uriBuilder.Uri;
        }

        public static string GetNextReportNumber(string Number)
        {
            string[] numberParts = Number.Split('-');
            if (numberParts.Length > 2)
            {
                numberParts[3] = Convert.ToString(Convert.ToInt64(numberParts[3]) + 1);
                return string.Join('-', numberParts);
            }
            else
            {
                return null;
            }
        }
        public static string GetNextSubReportNumber(string Number)
        {
            string[] numberParts = Number.Split('-');
            if (numberParts.Length > 2)
            {
                numberParts[5] = Convert.ToString(Convert.ToInt64(numberParts[5]) + 1);
                return string.Join('-', numberParts);
            }
            else
            {
                return null;
            }
        }
    }
}
