using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Utility.Utils
{
    class Constants
    {
        public static class TimeZone
        {
            public static string[] Inclusions = { "UTC", "Newfoundland", "Hawaii", "Atlantic", "Alaska", "Pierre", "Pacific", "Central", "Mountain", "Eastern", "Western" };
        }

        public static class JsonSerialize
        {
            public static string DateFormat = "yyyy-MM-ddTHH:mm:ss";

        }
    }
}
