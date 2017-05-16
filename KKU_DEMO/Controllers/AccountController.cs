using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.DAL;
using KKU_DEMO.Managers;
using KKU_DEMO.Models.Helpers;
using KKU_DEMO.Models.AuthModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;


namespace KKU_DEMO.Controllers
{
    public class AccountController : Controller
    {
        private static readonly KKUContext db = new KKUContext();
        private UserManager UserManager => HttpContext.GetOwinContext().GetUserManager<UserManager>();
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private ApplicationRoleManager RoleManager
            => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();

        [AuthorizeUser("SuperAdmin")]
        public ActionResult Register()
        {
            ViewBag.RoleList = RoleManager.Roles.ToList().Select(i => i.Name).ToList();
            ViewBag.Token = Guid.NewGuid().ToString("N").Substring(0, 6);

            ViewBag.CurrentPage = "user";
            return View();
        }

        [AuthorizeUser("SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            User user = new User {UserName = model.UserName, Name = model.Name, Password = model.Password};
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, model.Role);

                return RedirectToAction("Users", "Account");
            }
            else
            {
                foreach (string error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                ViewBag.RoleList = RoleManager.Roles.ToList().Select(i => i.Name).ToList();
                ViewBag.Token = Guid.NewGuid().ToString("N").Substring(0, 6);

                ViewBag.CurrentPage = "user";
            }


            return View(model);
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View("Login");
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    claim.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String));
                    claim.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name, ClaimValueTypes.String));

                    claim.AddClaim(
                        new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                            "OWIN Provider", ClaimValueTypes.String));

                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);

                    if (String.IsNullOrEmpty(returnUrl))
                    {
                       
                       
                            return RedirectToAction("About", "Home");
                        
                        
                    }
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        [AuthorizeUser("SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await UserManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] {"Пользователь не найден"});
            }
        }

        [Authorize]
        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult Users()
        {
            ViewBag.Roles = UserManager.GetRoles(User.Identity.GetUserId());
            ViewBag.CurrentPage = "user";
            return View(db.Users.OrderBy(u=>u.Name).ToList());
        }

        [Authorize]
        public ActionResult Generate()
        {
            ViewBag.Token = Guid.NewGuid().ToString("N").Substring(0, 6);
            return View();
        }
        [AuthorizeUser("SuperAdmin","Director")]
        public ActionResult Details(string id)
        {
            
            return View(db.Users.Find(id));
        }
    }
}