using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class NewsTypeRequest
    {

        [JsonProperty("news_type_name")]
        public string NewsTypeName { get; set; }

    }
}
