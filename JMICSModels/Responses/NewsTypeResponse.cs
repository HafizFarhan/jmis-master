using MTC.JMICS.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Responses
{
    public class NewsTypeResponse : MessageResponse
    {
        [JsonProperty(PropertyName = "News_Type_Id")]
        public virtual int NewsTypeId { get; set; }

        [JsonProperty(PropertyName = "NewsTypeName")]
        public virtual string NewsTypeName { get; set; }

        public NewsTypeResponse() : base(System.Net.HttpStatusCode.OK) { }
    }
    public class NewsTypeCollection : MessageResponse
    {

        List<NewsTypeResponse> Newstypes;
        [JsonProperty(PropertyName = "newsTypes")]
        public List<NewsTypeResponse> NewsTypes
        {
            get
            {
                if (Newstypes == null)
                    Newstypes = new List<NewsTypeResponse>();
                return Newstypes;
            }
            set
            {
                Newstypes = value;
            }
        }

        [JsonProperty(PropertyName = "cursor")]
        public CursorResponse Cursor
        {
            get;
            set;
        }
        public NewsTypeCollection() : base(System.Net.HttpStatusCode.OK) { }
    }

}
