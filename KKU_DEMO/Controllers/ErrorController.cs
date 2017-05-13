using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KKU_DEMO.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult RoleError()
        {
            ViewBag.Error = "Нет прав для просмотра данного контента";
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult ShiftError()
        {
            ViewBag.Error = "Смены отсутствуют";
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult ShiftDelete()
        {
            ViewBag.Error = "Смена удалена";
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult ShiftEditError()
        {
            ViewBag.Error = "Не удается изменить смену";
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult FileError()
        {
            Danger("Смена уже существует");
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult CustomError(string errorText)
        {
            ViewBag.Error = errorText;
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}