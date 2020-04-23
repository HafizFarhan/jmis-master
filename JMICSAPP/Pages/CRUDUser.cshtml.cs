using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JMICSAPP.Pages
{
    [Authorize]
    public class CRUDUserModel : PageModel
    {
        public SelectList SubscriberList { get; set; }

        public void OnGet()
        {

        }
    }
}