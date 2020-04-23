using JMICSAPP.Data;
using JMICSAPP.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppSettings.IntializeConfiguration(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("JMICSDBConnection")));


            services.AddIdentity<AppUser, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 60000000;
            });

            #region "Identity Settings"
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            #endregion

            services.AddMemoryCache();
            services.AddSingleton<MemCache>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseEndpointRouting();
            app.UseSignalR(routes =>
            {
                routes.MapHub<PushHub>("/pushmessages");
            });

            app.UseMvc();
            CreateRolesAndMasterUser(serviceProvider).Wait();
        }

        private async Task CreateRolesAndMasterUser(IServiceProvider serviceProvider)
        {
            try
            {
                RoleManager<IdentityRole> RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                UserManager<AppUser> UserManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                if (UserManager.Users.Count<AppUser>() == 0)
                {
                    string[] roleNames = { "ADMIN", "OPERATOR", "OIC", "ORO"};
                    IdentityResult roleResult;

                    foreach (var roleName in roleNames)
                    {
                        var roleExist = await RoleManager.RoleExistsAsync(roleName);                        
                        if (!roleExist)
                        {
                        //create the roles and seed them to the database
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }

                    //Here you could create a super user who will maintain the web app
                    var poweruser = new AppUser
                    {

                        UserName = "user1@jmicc.org",
                        Email = "user1@jmicc.org",
                        Subscriber_Id = 2
                    };

                    //Ensure you have these values in your appsettings.json file
                    string userPWD = "mtccsg";
                    var _user = await UserManager.FindByEmailAsync("user1@jmicc.org");

                    if (_user == null)
                    {
                        var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                        if (createPowerUser.Succeeded)
                        {
                            //here we tie the new user to the role
                            await UserManager.AddToRoleAsync(poweruser, "ADMIN");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
