using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Responses
{
    public class AAASIncidentResponse
    {
		[JsonProperty(PropertyName = "id")]
		public virtual int Id { get; set; }
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
		[JsonProperty(PropertyName = "createdOn")]
		public virtual DateTime CreatedOn { get; set; }
		[JsonProperty(PropertyName = "createdBy")]
		public virtual string CreatedBy { get; set; }
		[JsonProperty(PropertyName = "lastModifiedOn")]
		public virtual DateTime? LastModifiedOn { get; set; }
		[JsonProperty(PropertyName = "lastModifiedBy")]
		public virtual string LastModifiedBy { get; set; }
	}
}
