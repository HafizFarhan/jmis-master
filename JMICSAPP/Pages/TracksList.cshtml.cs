using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMICSAPP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JMICSAPP.Pages
{
    [Authorize]
    public class TracksListModel : PageModel
    {   
        public void OnGet()
        {

        }
    }
}