using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace JMICSAPP.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public async Task OnGetAsync()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync();

            LocalRedirect("~Login");
        }
    }
}

