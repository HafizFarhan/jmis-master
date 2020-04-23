using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Responses
{
    public class UserResponse : MessageResponse
    {
        public UserResponse() : base(System.Net.HttpStatusCode.OK) { }
    }

    public class UserCollection : MessageResponse
    {

        List<UserResponse> users;
        [JsonProperty(PropertyName = "users")]
        public List<UserResponse> Users
        {
            get
            {
                if (users == null)
                    users = new List<UserResponse>();
                return users;
            }
            set
            {
                users = value;
            }
        }

        [JsonProperty(PropertyName = "cursor")]
        public CursorResponse Cursor
        {
            get;
            set;
        }
        public UserCollection() : base(System.Net.HttpStatusCode.OK) { }
    }
}
