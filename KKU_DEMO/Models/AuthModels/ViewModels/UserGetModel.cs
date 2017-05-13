using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KKU_DEMO.Models.AuthModels;

namespace KKU_DEMO.Models.ViewModels
{
    public class UserGetModel
    {
        public IEnumerable<User> Users { get; set; }
    }
}