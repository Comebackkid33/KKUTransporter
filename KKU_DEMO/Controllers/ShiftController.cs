using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.DAL;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using KKU_DEMO.Models.Helpers;
using KKU_DEMO.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace KKU_DEMO.Controllers
{
    public class ShiftController : BaseController
    {
        private ShiftManager ShiftManager;

   

        public ShiftController()
        {
            ShiftManager = new ShiftManager();
        }

        [AuthorizeUser("SuperAdmin")]
        public ActionResult Create()
        {
            return View(ShiftManager.GetShiftCreateModel());
        }


        [AuthorizeUser("SuperAdmin", "Director")]
        [HttpPost]
        public ActionResult Create(ShiftCreateModel shiftCreateModel)
        {
            if (ModelState.IsValid)
            {
                if (ShiftManager.CheckExistense(shiftCreateModel))
                {
                    ShiftManager.Create(shiftCreateModel);
                    Success(
                        $"<b>Смена {shiftCreateModel.Number} от {shiftCreateModel.Date:d}</b> была успешно добавлена.",
                        true);
                    return RedirectToAction("Create", "Shift");
                }
                else
                {
                    Danger("Смена уже существует");
                    return View(ShiftManager.GetShiftCreateModel());
                }
            }
            else
            {
                Danger("Что-то пошло нетак, проверте данные");
                return View(ShiftManager.GetShiftCreateModel());
            }
        }

        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult Get()
        {
            
            return View(ShiftManager.GetShiftSearchModel());
        }

        [AuthorizeUser("SuperAdmin", "Director")]
        [HttpPost]
        public ActionResult GetShifts(ShiftSearchModel shiftSearchModel)
        {

            var shifts = ShiftManager.GetShifts(shiftSearchModel);
            
            if (shifts.Count != 0)
            {
              
                return PartialView(shifts);
            }
            else
                return RedirectToAction("ShiftError", "Error");
        }


        [AuthorizeUser("SuperAdmin", "Director")]
        public ActionResult Info(int id)
        {
           
            var shift = ShiftManager.GetById(id);

            if (shift != null)
            {
                
                return View(shift);
            }
            else
            {
                return RedirectToAction("ShiftDelete", "Error");
            }
        }

        [AuthorizeUser("SuperAdmin")]
        public ActionResult Delete(int id)
        {
            try
            {
                ShiftManager.Delete(id);
               // Success($"Смена  была успешно удалена.", true);
                return RedirectToAction("CustomError", "Error", new { errorText = "Смена удалена" });
            }
            catch (Exception e)
            {
                Danger("Ошибка удаления");
                return RedirectToAction("CustomError", "Error", new { errorText = "Ошибка удаления" });
            }
            
        }

        [AuthorizeUser("SuperAdmin")]
        public ActionResult Edit(int id)
        {
            return PartialView(ShiftManager.GetShiftCreateModel(id));
        }

        [HttpPost]
        [AuthorizeUser("SuperAdmin")]
        public ActionResult Edit(ShiftCreateModel sh)
        {
            
            try 
            {
               ShiftManager.Update(sh);
               Success($"<b>Смена {sh.Number} от {sh.Date:d}</b> была успешно изменена.", true);

                return RedirectToAction("Get", "Shift");
            }
            catch(Exception)
            {
                Danger("Смена с такими параметрами уже существует");
                return RedirectToAction("Get", "Shift");
            }
        }
    }
}