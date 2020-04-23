using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Requests
{
    public class COITypeRequest
    {
        [JsonProperty("coi_type_name")]
        public string COITypeName { get; set; }
    }
}
