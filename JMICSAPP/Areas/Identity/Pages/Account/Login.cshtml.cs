using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using JMICSAPP.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Principal;
using JMICSAPP.Models;
using MTC.JMICS.BL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using MTC.JMICS.Utility.Cache;

namespace JMICSAPP.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        #region "Getters and Setters"
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        public static AppUser user;
        public IHostingEnvironment _hostingEnvironment;
        MemCache _memCache;

        public LoginModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<LoginModel> logger, IHostingEnvironment env, IMemoryCache cache)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _hostingEnvironment = env;
            _memCache = new MemCache(cache);
        }

        [BindProperty]
        public LoginViewModel Login { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
        #endregion

        #region "Functions"
        private IActionResult RedirectToLocal(string returnUrl, string roleName)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                if (roleName == "Admin")
                {
                    return Redirect("/Admin/User");
                }
                else
                {
                    return Redirect("/User/UserProfile");
                }

            }
        }
        public void UserEventLogging(AppUser user, string eventDesc, int eventTypeId)
        {
            try
            {
                using (EventService eventService = new EventService())
                {
                    Event eventModel = new Event();
                    eventModel.EventTypeId = eventTypeId;
                    eventModel.SubscriberId = user.Subscriber_Id ?? 0;
                    eventModel.EventDescription = eventDesc;
                    eventModel.CreatedOn = DateTime.Now;
                    eventModel.CreatedBy = user.UserName;
                    eventService.Add(eventModel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Logging User Events " + Environment.NewLine + ex.Message);
            }
        }
        #endregion

        #region "Event Handlers"
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, ErrorMessage);

            returnUrl = returnUrl ?? Url.Content("~/");
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync();
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");
            string timeZone = "Pakistan Standard Time";

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Login.Email, Login.Password, Login.RememberMe, lockoutOnFailure: false);
                user = await _userManager.FindByEmailAsync(Login.Email);
                if (result.Succeeded)
                {
                    //if (!_hostingEnvironment.IsDevelopment())
                    //{
                    //    if (string.IsNullOrWhiteSpace(Login.Token))
                    //    {
                    //        ModelState.AddModelError(string.Empty, "Please provide token");
                    //        await HttpContext.SignOutAsync();
                    //        return Page();
                    //    }
                    //    else
                    //    {
                    //        EncryptorDecryptorEngine.DecryptString(Login.Token, out string decryptedString, out string errorMessage);
                    //        if (!string.IsNullOrWhiteSpace(errorMessage))
                    //        {
                    //            ModelState.AddModelError(string.Empty, errorMessage);
                    //            await HttpContext.SignOutAsync();
                    //            return Page();
                    //        }
                    //        else
                    //        {
                    //            string[] stringParts = decryptedString.Split("|");
                    //            if (stringParts.Length > 0)
                    //            {
                    //                timeZone = stringParts[1];
                                    
                    //                DateTime tokenDateTime = Convert.ToDateTime(stringParts[0]);
                    //                if (!(Common.GetLocalDateTime(timeZone, tokenDateTime) > Common.GetLocalDateTime(timeZone, DateTime.Now).AddMinutes(-1)))
                    //                {
                    //                    ModelState.AddModelError(string.Empty, "Token Expired. Please regenerate");
                    //                    await HttpContext.SignOutAsync();
                    //                    return Page();
                    //                }
                    //            }
                    //            else
                    //            {
                    //                ModelState.AddModelError(string.Empty, "Invalid Token");
                    //                await HttpContext.SignOutAsync();
                    //                return Page();
                    //            }
                    //        }
                    //    } 
                    //}

                    if(!MemCache.IsIncache("Timezone_" + user.Subscriber_Id))
                        MemCache.AddToCache("Timezone_" + user.Subscriber_Id, timeZone);

                    
                    _logger.LogInformation("User logged in.");
                    //user = await _userManager.FindByEmailAsync(Login.Email);

                    UserEventLogging(user, "User Signed In", Convert.ToInt32(EventTypes.User_Signed_In));

                    // Get the roles for the user
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("ADMIN"))
                    {
                        return LocalRedirect("~/Canvas");
                    }
                    else if (roles.Contains("OPERATOR"))
                    {
                        return LocalRedirect("~/Canvas");
                    }
                    else if (roles.Contains("OIC"))
                    {
                        return LocalRedirect("~/Canvas");
                    }
                    else if (roles.Contains("ORO"))
                    {
                        return LocalRedirect("~/Canvas");
                    }
                    else
                    {
                        return LocalRedirect("~Login");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("~/LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Login.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    UserEventLogging(user, "User account locked out", Convert.ToInt32(EventTypes.User_Account_Locked_Out));
                    return RedirectToPage("~/Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    UserEventLogging(user, "Invalid login attempt", Convert.ToInt32(EventTypes.Invalid_Login_Attempt));
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        #endregion
    }
}
