using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.DAL;
using KKU_DEMO.Helpers;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using KKU_DEMO.Models.Helpers;
using Microsoft.AspNet.Identity.Owin;

namespace KKU_DEMO.Controllers
{
    public class SensorController :  BaseController
    {
       
    //    private UserManager UserManager => HttpContext.GetOwinContext().GetUserManager<UserManager>();

        private SensorManager SensorManager;
        private FactoryManager FactoryManager;

        public SensorController()
        {
            SensorManager = new SensorManager();
            FactoryManager = new FactoryManager();
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.CurrentPage = "device";
            return View(SensorManager.GetAll());
        }

        [Authorize]
        public ActionResult Update()
        {
            return View(SensorManager.GetAll());
        }

        [Authorize]
        public ActionResult Info(int id)
        {
            return View(SensorManager.GetById(id));
        }

        [Authorize]
        [AuthorizeUser("SuperAdmin")]
        public ActionResult Create()
        {
            ViewBag.Token = Guid.NewGuid().ToString();
            ViewBag.FactoryList = FactoryManager.GetAll();
            ViewBag.CurrentPage = "device";
            return View();
        }

        [Authorize]
        [HttpPost]
        [HttpParamAction]
        [AuthorizeUser("SuperAdmin")]
        public ActionResult Create(Sensor sensor)
        {
            if (ModelState.IsValid)
            {
               SensorManager.Create(sensor);
                return RedirectToAction("Index", "Sensor");
            }
            else
            {
                ViewBag.Token = sensor.Token;
                return View();
            }
        }

        [Authorize]
        [HttpParamAction]
        public ActionResult Generate()
        {
            ViewBag.Token = Guid.NewGuid().ToString();
            return View();
        }
        [Authorize]
        public ActionResult Download(int id)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~") + "\\log\\sensor_"+id+"_Log.txt";
            string fileName = String.Format("Sensor_{0}_{1}_Log.txt",id,DateTime.Now);
            string content_type = "application/txt";
             
            if (System.IO.File.Exists(path))
            {
                return File(path, content_type, fileName);
            }
            else
            {
                Danger("Файл не найден");
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }
    }
}