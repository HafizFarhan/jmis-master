using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace MTC.JMICS.Models.Responses
{
    public class MessageResponse
    {

        #region Fields
        /// Response Http Status Code
        private HttpStatusCode statusCode;
        [JsonIgnore]
        public HttpStatusCode StatusCode { get { return this.statusCode; } }

        #endregion

        #region Constructors
        /// Initialize the object
        public MessageResponse(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }

        #endregion

        #region Methods

        /// Get the HttpResponseMessage
        

        #endregion

    }

    public class BasicMessageResponse : MessageResponse
    {

        #region Fields & Properties

        /// <summary>
        /// Status code from the response
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public HttpStatusCode Code { get { return base.StatusCode; } }

        private string message;
        /// <summary>
        /// Message from the response
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get { return message; } set { message = value; } }


        private object informations;
        /// <summary>
        /// Message from the response
        /// </summary>
        [JsonProperty(PropertyName = "informations")]
        public object Informations { get { return informations; } set { informations = value; } }


        #endregion

        #region Constructor

        public BasicMessageResponse(HttpStatusCode statusCode, string message, object informations)
            : base(statusCode)
        {
            this.message = message;
            this.informations = informations;
        }
        public BasicMessageResponse() : base(System.Net.HttpStatusCode.OK) { }
        #endregion
    }

    public class LoginErrorMessageResponse : MessageResponse
    {

        #region Fields & Properties

        [JsonProperty(PropertyName = "code")]
        public HttpStatusCode Code { get { return base.StatusCode; } }

        string message;
        [JsonProperty(PropertyName = "message")]
        public string Message { get { return this.message; } set { this.message = value; } }

        object error;
        [JsonProperty(PropertyName = "errors")]
        public object Errors { get { return this.error; } set { this.error = value; } }

        bool isLastAttempt;
        [JsonProperty(PropertyName = "isLastAttempt")]
        public bool IsLastAttempt { get { return this.isLastAttempt; } }

        bool isBlocked;
        [JsonProperty(PropertyName = "isBlocked")]
        public bool IsBlocked { get { return this.isBlocked; } }

        #endregion


        #region Constructor

        public LoginErrorMessageResponse(HttpStatusCode statusCode, string message, bool isLastAttempt, bool isBlocked, object errors)
            : base(statusCode)
        {
            this.message = message;
            this.isLastAttempt = isLastAttempt;
            this.isBlocked = isBlocked;
            this.error = errors;
        }

        #endregion
    }
}
