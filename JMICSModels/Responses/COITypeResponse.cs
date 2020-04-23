using MTC.JMICS.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Responses
{
    public class COITypeResponse : MessageResponse
    {
        [JsonProperty(PropertyName = "COI_Type_Id")]
        public virtual int COITypeId { get; set; }

        [JsonProperty(PropertyName = "COITypeName")]
        public virtual string COITypeName { get; set; }

        public COITypeResponse() : base(System.Net.HttpStatusCode.OK) { }
    }
    public class COITypeCollection : MessageResponse
    {

        List<COITypeResponse> COItypes;
        [JsonProperty(PropertyName = "COITypes")]
        public List<COITypeResponse> CoiTypes
        {
            get
            {
                if (COItypes == null)
                    COItypes = new List<COITypeResponse>();
                return COItypes;
            }
            set
            {
                COItypes = value;
            }
        }

        [JsonProperty(PropertyName = "cursor")]
        public CursorResponse Cursor
        {
            get;
            set;
        }
        public COITypeCollection() : base(System.Net.HttpStatusCode.OK) { }
    }

}
