using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
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
        public ActionResult _ByPeriod(string id)
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
                    return View("~/Views/Stat/_ByPeriod.cshtml", stat);
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
        [HttpPost]
        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult ExportToExcel(StatModel statModel)
        {
            GridView gridview = new GridView();
            gridview.DataSource = statModel.ExelTable;
            gridview.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            // set the header
            Response.AddHeader("content-disposition", "attachment;filename = itfunda.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            // create HtmlTextWriter object with StringWriter
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    // render the GridView to the HtmlTextWriter
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            return View("ByPeriod");
        }

    }

    }


