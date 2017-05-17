using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.DAL;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using KKU_DEMO.Models.Helpers;

namespace KKU_DEMO.Controllers
{
    public class StatController : BaseController
    {
        private static readonly KKUContext db = new KKUContext();

        private StatManager StatManager;

        public StatController()
        {
            StatManager = new StatManager();
        }

        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult ByDay()
        {

            return View();
        }

        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult GetByDay(string id)
        {

            double totalWeight = 0;
            int downTime = 0;
            double productionPct = 0;
            var d = DateTime.Parse(id + " 00:00:00.000", System.Globalization.CultureInfo.InvariantCulture);


            var shifts =
                db.Shift.Where(c => (c.Date == d && c.State == StateEnum.CLOSED.ToString())).Select(c => c).ToList();

            if (shifts.Count != 0)
            {

                foreach (var s in shifts)
                {
                    totalWeight = totalWeight + s.TotalShiftWeight;
                    downTime = downTime + s.DownTime;
                    productionPct = productionPct + s.ProductionPct;
                }

                productionPct = productionPct/shifts.Count;

                return View(new StatModel()
                {
                    Date = d.ToShortDateString(),
                    TotalWeight = totalWeight,
                    DownTime = downTime,
                    ProductionPct = productionPct
                });
            }


            else
                return RedirectToAction("ShiftError", "Error");

        }

        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult ByPeriod()
        {

            return View();
        }

        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult GetByPeriod(string start, string end)
        {
            
            try
            {
                var stat = StatManager.ByPeriod(start, end);
                if (stat != null)
                {
                    return View("~/Views/Stat/GetByDay.cshtml", stat);
                }
                else
                {
                    throw new Exception("Нет смен в указанном периоде");
                }
                
            }
            catch (Exception)
            {

                return RedirectToAction("ShiftError", "Error");
            }


        }

    }


}