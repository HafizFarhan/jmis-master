using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Responses
{
    public class SubscriberResponse : MessageResponse
    {
        [JsonProperty(PropertyName = "subscriberID")]
        public virtual int SubscriberID { get; set; } // int, not null
        [JsonProperty(PropertyName = "subscriberName")]
        public virtual string SubscriberName { get; set; } // nvarchar(50), null
        [JsonProperty(PropertyName = "address")]
        public virtual string Address { get; set; } // nvarchar(500), null
        [JsonProperty(PropertyName = "stateCode")]
        public virtual string StateCode { get; set; } // nvarchar(2), null
        [JsonProperty(PropertyName = "city")]
        public virtual string City { get; set; } // nvarchar(100), null
        [JsonProperty(PropertyName = "zip")]
        public virtual string Zip { get; set; } // nvarchar(10), null
        [JsonProperty(PropertyName = "email")]
        public virtual string Email { get; set; } // nvarchar(200), null
        [JsonProperty(PropertyName = "phone")]
        public virtual string Phone { get; set; } // nvarchar(200), null
        [JsonProperty(PropertyName = "subscriberTimeZone")]
        public virtual string SubscriberTimeZone { get; set; } // nvarchar(50), null
        [JsonProperty(PropertyName = "picturePath")]
        public virtual string PicturePath { get; set; } // nvarchar(500), null
        [JsonProperty(PropertyName = "pictureName")]
        public virtual string PictureName { get; set; } // nvarchar(500), null
        [JsonProperty(PropertyName = "uniquePictureName")]
        public virtual string UniquePictureName { get; set; } // nvarchar(500), null
        [JsonProperty(PropertyName = "isApproved")]
        public virtual bool IsApproved { get; set; } // bit, not null

        public SubscriberResponse() : base(System.Net.HttpStatusCode.OK) { }
    }

    public class SubscriberCollection : MessageResponse
    {

        List<SubscriberResponse> subscribers;
        [JsonProperty(PropertyName = "subscribers")]
        public List<SubscriberResponse> Subscribers
        {
            get
            {
                if (subscribers == null)
                    subscribers = new List<SubscriberResponse>();
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
        public SubscriberCollection() : base(System.Net.HttpStatusCode.OK) { }
    }
}
