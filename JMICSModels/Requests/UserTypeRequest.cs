using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class UserTypeRequest
    {
        [JsonProperty(PropertyName = "userTypeId")]
        public virtual int UserTypeId { get; set; } // int, not null
    }
}
