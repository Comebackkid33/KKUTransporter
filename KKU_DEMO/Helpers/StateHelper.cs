using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.Managers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace KKU_DEMO.Helpers
{
    public static class StateHelper
    {
        public static MvcHtmlString Translate(this HtmlHelper html, string str, bool isIncident = false)
        {
            switch (str)
            {
                case "STOP":
                    return new MvcHtmlString("Остановлен");
                case "OK":
                    return new MvcHtmlString("Работает");
                case "NOLOAD":
                    return new MvcHtmlString("Холостой ход");
                case "OFF":
                    return new MvcHtmlString("Отключен");
                case "INPROCESS":
                    return new MvcHtmlString("Выполняется");
                case "ASSIGNED":
                    return new MvcHtmlString("Заведена");
                case "CLOSED":
                    if (isIncident)
                        return new MvcHtmlString("Закрыт");
                    else
                        return new MvcHtmlString("Закончена");

                case "OPENED":
                    return new MvcHtmlString("Открыт");
            }

            return MvcHtmlString.Empty;
        }

        public static string TranslateRole(string role)
        {
            switch (role)
            {
                case "Master":
                    return "Мастер";
                case "Director":
                    return "Директор";
                case "SuperAdmin":
                    return "Администратор";
            }
            return "";
        }
    }
}

