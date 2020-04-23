using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Requests
{
    public class AAASIncidentRequest
    {
		[JsonProperty(PropertyName = "userContactNumber")]
		public virtual string UserContactNumber { get; set; }
		[JsonProperty(PropertyName = "incidentType")]
		public virtual string IncidentType { get; set; }
		[JsonProperty(PropertyName = "description")]
		public virtual string Description { get; set; }
		[JsonProperty(PropertyName = "latitude")]
		public virtual decimal? Latitude { get; set; }
		[JsonProperty(PropertyName = "longitude")]
		public virtual decimal? Longitude { get; set; }
	}
}
