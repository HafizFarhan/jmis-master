using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Responses
{
    public class ErrorMessageResponse : MessageResponse
    {
        #region Fields & Properties

        /// <summary>
        /// Status code from the response
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public HttpStatusCode Code { get { return base.StatusCode; } }

        string message;
        /// <summary>
        /// Message from the response
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get { return this.message; } set { this.message = value; } }

        object error;
        /// <summary>
        /// Message from the response
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public object Errors { get { return this.error; } set { this.error = value; } }

        #endregion

        #region Constructor

        public ErrorMessageResponse(HttpStatusCode statusCode, string message, object errors)
            : base(statusCode)
        {
            this.message = message;
            error = errors;
        }

        #endregion
    }
}
