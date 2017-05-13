using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.Managers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace KKU_DEMO.Helpers
{
    public static class IdentityHelpers
    {
        public static MvcHtmlString GetUserRoles(this HtmlHelper html, string id)
        {
            UserManager mgr = HttpContext.Current
                .GetOwinContext().GetUserManager<UserManager>();
            var roles = mgr.GetRoles(id);
            return new MvcHtmlString(roles[0]);
        }
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            UserManager mgr = HttpContext.Current
                 .GetOwinContext().GetUserManager<UserManager>();
            return new MvcHtmlString(mgr.FindByIdAsync(id).Result.UserName);
        }
        public static MvcHtmlString GetName(this HtmlHelper html, string id)
        {
            UserManager mgr = HttpContext.Current
                 .GetOwinContext().GetUserManager<UserManager>();
            return new MvcHtmlString(mgr.FindByIdAsync(id).Result.Name);
        }
    }
}