using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MTC.JMICS.Utility.Utils
{
    public class AppSettings
    {
        public static IConfiguration Configuration { get; private set; }
        public static void IntializeConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
