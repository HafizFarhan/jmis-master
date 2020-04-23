using MTC.JMICS.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Responses
{
    public class NatureOfThreatResponse : MessageResponse
    {
        [JsonProperty(PropertyName = "Threat_Id")]
        public virtual int ThreatId { get; set; }

        [JsonProperty(PropertyName = "threatName")]
        public virtual string ThreatName { get; set; }

        public NatureOfThreatResponse() : base(System.Net.HttpStatusCode.OK) { }
    }

    public class NatureOfThreatCollection : MessageResponse
    {

        List<NatureOfThreatResponse> subscribers;
        [JsonProperty(PropertyName = "NatureOfThreat")]
        public List<NatureOfThreatResponse> Subscribers
        {
            get
            {
                if (subscribers == null)
                    subscribers = new List<NatureOfThreatResponse>();
                return subscribers;
            }
            set
            {
                subscribers = value;
            }
        }

        [JsonProperty(PropertyName = "cursor")]
        public CursorResponse Cursor
        {
            get;
            set;
        }
        public NatureOfThreatCollection() : base(System.Net.HttpStatusCode.OK) { }
    }

}
