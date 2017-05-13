using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.Managers;
using KKU_DEMO.Models.AuthModels;
using Microsoft.AspNet.Identity;

namespace KKU_DEMO.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult About()
        {
            if (User.Identity.IsInRole("Master"))
            {
                return RedirectToAction("GetByUserId", "Incident",new {id = User.Identity.GetUserId()});
            }
            return View();
        }
    }
}