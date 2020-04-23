using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class LoginRequest
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required. ")]
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required. ")]
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "localip")]
        public string LocalIP { get; set; }
    }
}
