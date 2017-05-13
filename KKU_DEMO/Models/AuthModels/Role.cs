using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KKU_DEMO.Models.AuthModels
{
    public class Role : IdentityRole
    {
        public Role() : base() { }
        public Role(string name) : base(name) { }
       
    }
}