using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Data
{
    public class AppUser : IdentityUser
    {
        public int? Subscriber_Id { get; set; }

    }
}
