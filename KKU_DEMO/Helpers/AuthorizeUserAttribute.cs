using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using KKU_DEMO.Managers;
using KKU_DEMO.Models.AuthModels;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace KKU_DEMO.Models.Helpers
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        private String[] _roles;
        public AuthorizeUserAttribute(params String[] roles)
        {
            _roles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            return (_roles).Any(r => HttpContext.Current.User.Identity.IsInAnyRoles(r));
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Error",
                                action = "RoleError"
                            })
                        );
        }
    }
}