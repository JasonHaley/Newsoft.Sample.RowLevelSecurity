using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newsoft.Sample.RowLevelSecurity.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newsoft.Sample.RowLevelSecurity.DAL;


namespace Newsoft.Sample.RowLevelSecurity.Web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private static Guid tenant1Id = new Guid("F06FE619-21CC-E511-8113-005056BE3E20");
        private static Guid tenant2Id = new Guid("F16FE619-21CC-E511-8113-005056BE3E20");

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            // Only needed on first load of application to add demo users
            await CreateDemoUsersIfNeeded();

            // once the user is logged in, add chuck norris and get listing of employees
            if (User.Identity.IsAuthenticated)
            {
                var context = new NorthwindContext();

                // only adds chuck if he isn't there already for the current user's tenant
                await AddChuckNorrisToMyTenant(context);

                // get list of employees for this tenant
                ViewBag.Employees = context.Employees.ToList();
            }

            return View();
        }

        private async Task CreateDemoUsersIfNeeded()
        {
            // only add if there are no users
            if (!UserManager.Users.Any())
            {
                await CreateDemoUser("jack@company.com", "Password123!", tenant1Id.ToString());
                await CreateDemoUser("jill@company.com", "Password123!", tenant2Id.ToString());
            }
        }
        
        private async Task AddChuckNorrisToMyTenant(NorthwindContext context)
        {
            if (!context.Employees.Any(e => e.FirstName == "Chuck" && e.LastName == "Norris"))
            {
                //Example of adding employee to tenant 
                var chuck = new Employee();
                chuck.FirstName = "Chuck";
                chuck.LastName = "Norris";
                chuck.Address = "Everywhere";
                chuck.EmployeeId = new Random().Next();
                context.Employees.Add(chuck);
                await context.SaveChangesAsync();
            }
        }

        private async Task CreateDemoUser(string email, string password, string tenantId)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Add tenant claim to user and update
                user.Claims.Add(new IdentityUserClaim()
                {
                    ClaimType = "TenantId",
                    ClaimValue = tenantId
                });

                await UserManager.UpdateAsync(user);
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}