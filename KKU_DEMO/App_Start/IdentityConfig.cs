using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Managers;
using KKU_DEMO.Models.AuthModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace KKU_DEMO.App_Start
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new KKUContext());
            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

         
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = System.TimeSpan.FromHours(2),
                LoginPath = new PathString("/Account/LogIn"),
            });
        }

    }
}