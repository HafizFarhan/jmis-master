using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Utility.Security
{
    public class SecurityManager
    {
        public static Dictionary<string, string> PasswordPolicy()
        {
            Dictionary<string, string> dicPP = new Dictionary<string, string>();
            string[] pp = AppSettings.Configuration.GetSection("ApplicationSettings")["PasswordPolicy"].ToString().Split(',');
            foreach (var val in pp)
            {
                if (val.Split(':')[0] == "MN") dicPP.Add("Length", val.Split(':')[1]);
                if (val.Split(':')[0] == "SC") dicPP.Add("Special", val.Split(':')[1]);
                if (val.Split(':')[0] == "NC") dicPP.Add("Numeric", val.Split(':')[1]);
                if (val.Split(':')[0] == "UC") dicPP.Add("Upper", val.Split(':')[1]);
                if (val.Split(':')[0] == "LC") dicPP.Add("Lower", val.Split(':')[1]);
                if (val.Split(':')[0] == "EX") dicPP.Add("Expiry", val.Split(':')[1]);
            }
            return dicPP;
        }
    }
}