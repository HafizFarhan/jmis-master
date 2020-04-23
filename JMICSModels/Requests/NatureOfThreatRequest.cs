using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Requests
{
    public class NatureOfThreatRequest
    {
        [JsonProperty("threat_name")]
        public string ThreatName { get; set; }
    }
}
