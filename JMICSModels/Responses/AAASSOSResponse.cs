using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Responses
{
    public class AAASSOSResponse
    {
        [JsonProperty(PropertyName ="id")]
        public virtual int Id { get; set; }
        [JsonProperty(PropertyName ="userContactNumber")]
        public virtual string UserContactNumber { get; set; }
        [JsonProperty(PropertyName ="userIMEI")]
        public virtual decimal? UserIMEI { get; set; }
        [JsonProperty(PropertyName ="latitude")]
        public virtual decimal? Latitude { get; set; }
        [JsonProperty(PropertyName ="longitude")]
        public virtual decimal? Longitude { get; set; }
        [JsonProperty(PropertyName ="address")]
        public virtual string Address { get; set; }
        [JsonProperty(PropertyName ="createdOn")]
        public virtual DateTime CreatedOn { get; set; }
        [JsonProperty(PropertyName ="createdBy")]
        public virtual string CreatedBy { get; set; }
        [JsonProperty(PropertyName ="lastModifiedOn")]
        public virtual DateTime? LastModifiedOn { get; set; }
        [JsonProperty(PropertyName ="lastModifiedBy")]
        public virtual string LastModifiedBy { get; set; }
    }
}
