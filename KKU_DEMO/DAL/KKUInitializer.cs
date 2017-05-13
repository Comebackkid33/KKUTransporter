using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using KKU_DEMO.Models.AuthModels;
using KKU_DEMO.Models.DataModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KKU_DEMO.DAL
{
    public class KKUInitializer : DropCreateDatabaseIfModelChanges<KKUContext>
    {
        protected override void Seed(KKUContext context)
        {
            context.Database.Initialize(true);

            var userManager = new UserManager(new UserStore<User>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var role1 = new IdentityRole { Name = "SuperAdmin" };
            var role2 = new IdentityRole { Name = "Master" };
            var role3 = new IdentityRole { Name = "Director" };

            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);

            var admin = new User { UserName = "Konstantin_Kurganov" ,Name = "Константин Курганов"};
            string password = "29069547a";
            var result = userManager.Create(admin, password);

            // если создание пользователя прошло успешно
            if (result.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
                
            }

            var factory = new Factory { Name = "Цех 3" };
            context.Factory.Add(factory);

            var sensor = new Sensor { Name = "Вход" ,FactoryId = 1, Token = "XXXX-XXXX-XXXX-XXXX", TotalWeight = 0.0,CurWeight = "15/12/16 19:56:59 4233 КГ",DownTime = 0};
            context.Sensor.Add(sensor);
        
            
            base.Seed(context);


        }
    }
}