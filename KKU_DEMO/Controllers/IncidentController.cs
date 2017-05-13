using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.DAL;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using KKU_DEMO.Models.AuthModels;
using Microsoft.AspNet.Identity;

namespace KKU_DEMO.Controllers
{
    public class IncidentController : BaseController
    {
        private IncidentManager IncidentManager;
        public IncidentController() : base()
        {
            IncidentManager = new IncidentManager();
        }
        // GET: Incident
        public ActionResult Index()
        {
           
            ViewBag.Title = "Инциденты:";
            return View(IncidentManager.GetAllIncidents());
        }
        //
        public ActionResult GetByUserId(string id)
        {
            ViewBag.Title = "Инциденты в Ваши смены:";
            return View("Index",IncidentManager.GetIncidentsByUser(id));
        }
        public ActionResult SetCause(int id)
        {
            KKUContext db = new KKUContext();
            ViewBag.CauseList = db.Cause.ToList();
            
            return View(IncidentManager.GetIncident(id));
        }
        [HttpPost]
        public ActionResult SetCause( Incident incident)
        {   

            IncidentManager.UpdateIncident(incident);
            Success($"<b>Инцидент  от {incident.Time:d}</b> был успешно закрыт.", true);
            if (User.Identity.IsInRole("Master"))
            {
                return RedirectToAction("GetByUserId", new { id = User.Identity.GetUserId()});
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
    }
}