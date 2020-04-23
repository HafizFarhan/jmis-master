using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Responses
{

    public class LoginResponse : MessageResponse
    {
        [JsonProperty(PropertyName = "user")]
        public UserLoginResponse User { get; set; }
        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "session")]
        public string Session { get; set; }

        #region Constructor
        public LoginResponse(
                                        int userID,
                                        string userlogin,
                                        string useremail,
                                        string userFirstName,
                                        string userLastName,
                                        string userShortName,
                                        bool isFirstLogin,
                                        bool isMasterUser,
                                        int userTypeID,
                                        int subscriberID,
                                        string subscriber,
                                        string token,
                                        bool? isUserLoginVerified,
                                        bool? isSubscriberApproved
                                           ) : base(HttpStatusCode.OK)
        {
            User = new UserLoginResponse();
            User.UserID = userID;
            User.UserLogin = userlogin;
            User.email = useremail;
            User.FirstName = userFirstName;
            User.LastName = userLastName;
            User.ShortName = userShortName;
            User.IsFirstLogin = isFirstLogin;
            User.IsMasterUser = isMasterUser;
            User.SubscriberID = subscriberID;
            User.SubscriberName = subscriber;
            User.IsUserLoginVerified = isUserLoginVerified;
            User.IsSubscriberApproved = isSubscriberApproved;
            User.UserTypeID = userTypeID;
            AccessToken = token;
            Session = Guid.NewGuid().ToString();
        }
        #endregion

    }

    public class UserLoginResponse
    {
        [JsonProperty(PropertyName = "userID")]
        public int UserID { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }

        [JsonProperty(PropertyName = "userLogin")]
        public string UserLogin { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "shortName")]
        public string ShortName { get; set; }

        [JsonProperty(PropertyName = "isFirstLogin")]
        public bool IsFirstLogin { get; set; }

        [JsonProperty(PropertyName = "isMasterUser")]
        public bool IsMasterUser { get; set; }

        [JsonProperty(PropertyName = "vendorID")]
        public int VendorID { get; set; }

        [JsonProperty(PropertyName = "clientID")]
        public int ClientID { get; set; }

        [JsonProperty(PropertyName = "subscriberID")]
        public int SubscriberID { get; set; }
        [JsonProperty(PropertyName = "subscriberName")]
        public string SubscriberName { get; set; }

        [JsonProperty(PropertyName = "userTypeID")]
        public int UserTypeID { get; set; }

        [JsonProperty(PropertyName = "isUserLoginVerified")]
        public bool? IsUserLoginVerified { get; set; }

        [JsonProperty(PropertyName = "isSubscriberApproved")]
        public bool? IsSubscriberApproved { get; set; }
    }


}
