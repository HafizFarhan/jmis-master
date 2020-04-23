using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Responses
{
    public class UserTypeResponse : MessageResponse
    {
        public UserTypeResponse() : base(System.Net.HttpStatusCode.OK) { }
    }

    public class UserTypeCollection : MessageResponse
    {

        List<UserTypeResponse> userTypes;
        [JsonProperty(PropertyName = "userTypes")]
        public List<UserTypeResponse> UserTypes
        {
            get
            {
                if (userTypes == null)
                    userTypes = new List<UserTypeResponse>();
                return userTypes;
            }
            set
            {
                userTypes = value;
            }
        }

        [JsonProperty(PropertyName = "cursor")]
        public CursorResponse Cursor
        {
            get;
            set;
        }
        public UserTypeCollection() : base(System.Net.HttpStatusCode.OK) { }
    }
}
