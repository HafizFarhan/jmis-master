using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class UserRequest
    {
        [JsonProperty(PropertyName = "userId")]
        public virtual int UserID { get; set; } // int, not null
    }
}
