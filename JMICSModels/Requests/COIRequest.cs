using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class COIRequest
    {
        [JsonProperty("creation_date")]
        public string GenerationDate { get; set; }

        //[JsonProperty("subscriber_id")]
        //public long SubscriberId { get; set; }

        [JsonProperty("address_to")]
        public string AdderssedTo { get; set; }

        [JsonProperty("coi_type_id")]
        public long COITypeId { get; set; }

        [JsonProperty("nature_of_threat")]
        public long NatureOfThreat { get; set; }

        [JsonProperty("area_information")]
        public string AreaInformation { get; set; }

        [JsonProperty("latitude")]
        public string COILatitude { get; set; }

        [JsonProperty("longitude")]
        public string COILongitude { get; set; }

        [JsonProperty("description")]
        public string COIInformation { get; set; }

        [JsonProperty("incident_occured_date")]
        public string IncidentOccuredOn { get; set; }
    }
}
