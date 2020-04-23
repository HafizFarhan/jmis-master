using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Requests
{
    public class AAASSOSRequest
    {
        [JsonProperty(PropertyName = "userContactNumber")]
        public virtual string UserContactNumber { get; set; }
        [JsonProperty(PropertyName = "userIMEI")]
        public virtual decimal? UserIMEI { get; set; }
        [JsonProperty(PropertyName = "latitude")]
        public virtual decimal? Latitude { get; set; }
        [JsonProperty(PropertyName = "longitude")]
        public virtual decimal? Longitude { get; set; }
        [JsonProperty(PropertyName = "address")]
        public virtual string Address { get; set; }
    }
}
