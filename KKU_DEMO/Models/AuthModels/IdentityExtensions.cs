using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using KKU_DEMO.Managers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace KKU_DEMO.Models.AuthModels
{
    public static class IdentityExtensions
    {
        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                var id = ci.FindFirst(ClaimTypes.NameIdentifier);
                if (id != null)
                {
                    return (T)Convert.ChangeType(id.Value, typeof(T), CultureInfo.InvariantCulture);
                }
            }
            return default(T);
        }
        public static string GetName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var ci = identity as ClaimsIdentity;
            string name = "";
            if (ci != null)
            {
                var id = ci.FindFirst(ClaimsIdentity.DefaultNameClaimType);
                if (id != null)
                    name = id.Value;
            }
            return name;
        }
        public static bool IsInRole(this IIdentity identity, string role)
        {
            return HttpContext.Current.GetOwinContext()
                .GetUserManager<UserManager>()
                .IsInRole(identity.GetUserId(), role);
        }
        public static bool IsInAnyRoles(this IIdentity identity,  params string[] roles)
        {
            return roles.Any(r => HttpContext.Current.GetOwinContext().GetUserManager<UserManager>().IsInRole(identity.GetUserId(), r));
        }
    }
}