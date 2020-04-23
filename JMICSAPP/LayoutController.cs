using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMICSAPP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JMICSAPP
{
    [Route("[controller]/[action]")]
    public class LayoutController : Controller
    {
        public AppUser user;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public LayoutController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Signout()
        {
            _signInManager.SignOutAsync();
            return Redirect("/Identity/Account/Login");
        }
    }
}
