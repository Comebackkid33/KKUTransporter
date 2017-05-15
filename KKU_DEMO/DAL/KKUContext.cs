using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KKU_DEMO.Models;
using KKU_DEMO.Models.AuthModels;
using KKU_DEMO.Models.DataModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KKU_DEMO.DAL
{
    public class KKUContext:  IdentityDbContext<User>

    {
        public KKUContext() : base("u0302353_KovrovKU_Test")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }
        public DbSet<Sensor> Sensor { get; set; }
        public DbSet<Cause> Cause { get; set; }
        public DbSet<Factory> Factory { get; set; }
        public DbSet<Incident> Incident { get; set; }
        public DbSet<Shift> Shift{ get; set; }

   
    }

}