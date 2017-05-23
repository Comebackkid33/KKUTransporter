using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using KKU_DEMO.DAL;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using KKU_DEMO.Models.Helpers;
using OfficeOpenXml;

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

        //[AuthorizeUser("SuperAdmin", "Director")]
        //public ActionResult _ByPeriod(string id)
        //{

        //    double totalWeight = 0;
        //    int downTime = 0;
        //    double productionPct = 0;
        //    var d = DateTime.Parse(id + " 00:00:00.000", System.Globalization.CultureInfo.InvariantCulture);


        //    var shifts =
        //        db.Shift.Where(c => (c.Date == d && c.State == StateEnum.CLOSED.ToString())).Select(c => c).ToList();

        //    if (shifts.Count != 0)
        //    {

        //        foreach (var s in shifts)
        //        {
        //            totalWeight = totalWeight + s.TotalShiftWeight;
        //            downTime = downTime + s.DownTime;
        //            productionPct = productionPct + s.ProductionPct;
        //        }

        //        productionPct = productionPct/shifts.Count;

        //        return View(new StatModel()
        //        {
        //            Date = d.ToShortDateString(),
        //            TotalWeight = totalWeight,
        //            DownTime = downTime,
        //            ProductionPct = productionPct
        //        });
        //    }


        //    else
        //        return RedirectToAction("ShiftError", "Error");

        //}

        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult ByPeriod()
        {
            
            return View(StatManager.GetStatModel());
        }

        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult GetByPeriod(string start, string end, int factoryId)
        {
            
            try
            {
                var stat = StatManager.ByPeriod(start, end,factoryId);
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

            using (ExcelPackage pck = new ExcelPackage())
            {
                var thirstList = pck.Workbook.Worksheets.Add("Выработка");
                //Create the worksheet
                var ws = pck.Workbook.Worksheets.Add("Инциденты");
                //Load the datatable into the sheet, starting from cell A1.
                // Print the   column names on row 1
                var table1 = thirstList.Cells["A1"].LoadFromCollection(statModel.ExelTable, true);
                var table2 = ws.Cells["A1"].LoadFromCollection(statModel.IncidentTable, true);

                //Write it back to the client
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Статистика за "+statModel.Date+".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
            }

            return new EmptyResult(); 
        }
     


        

    }

    }


