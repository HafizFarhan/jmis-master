using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class AppUserViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email Required")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Username Required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Subscriber Required")]
        public int SubscriberId { get; set; }
        [Required(ErrorMessage = "Role Required")]
        public string Role { get; set; }

    }
}
