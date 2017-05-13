using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KKU_DEMO.Models.AuthModels
{
    public class User : IdentityUser
    {
        public string Password { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
        public int ChatId { get; set; }


    }

}