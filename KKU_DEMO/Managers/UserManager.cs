using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models.AuthModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace KKU_DEMO.Managers
{
    public class UserManager : UserManager<User>
    {

        public UserManager(IUserStore<User> store)
            : base(store)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new KKUContext()));
        }

        // this method is called by Owin therefore best place to configure your User Manager
        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager = new UserManager(
                new UserStore<User>(context.Get<KKUContext>()));

            // optionally configure your manager
            // ...

            return manager;
        }

    }
}

