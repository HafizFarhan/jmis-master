using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class NewsRequest
    {

        [JsonProperty("news_type_id")]
        public long NewsTypeId { get; set; }

        [JsonProperty("heading")]
        public string NewsHeading { get; set; }

        [JsonProperty("reported_to")]
        public string ReportedTo { get; set; }

        [JsonProperty("description")]
        public string NewsDescription { get; set; }



    }
}
