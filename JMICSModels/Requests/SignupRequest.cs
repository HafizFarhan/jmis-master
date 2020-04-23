using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class SignupRequest
    {
        [JsonProperty(PropertyName = "firstName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required. ")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required. ")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email  is required. ")]
        //[EmailValidation(ErrorMessage = "Please provide a valid email.")]
        public string Email { get; set; }

        [Required]
        [JsonProperty(PropertyName = "phone")]
        [StringLength(200)]
        public string Phone { get; set; }
        
        [JsonProperty(PropertyName = "userLogin")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Login  is required. ")]
        [RegularExpression(@"^[A-Za-z0-9@_.-]+$", ErrorMessage = "Please provide valid user Login")]
        public string UserLogin { get; set; }
      
        [JsonProperty(PropertyName = "password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required. ")]
        //[PasswordValidation(ErrorMessage = "Please provide a valid Password.")]
        public string Password { get; set; }


        [JsonProperty(PropertyName = "subscriberId")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Subscriber Id is required. ")]
        //[PasswordValidation(ErrorMessage = "Please provide a valid Password.")]
        public int SubscriberId { get; set; }


        [JsonProperty(PropertyName = "userTypeId")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User type Id is required. ")]
        //[PasswordValidation(ErrorMessage = "Please provide a valid Password.")]
        public int UserTypeId { get; set; }

    }
}
