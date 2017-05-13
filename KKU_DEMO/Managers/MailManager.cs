using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using KKU_DEMO.Models;

namespace KKU_DEMO.Managers
{
    public class MailManager
    {
        private string address;
        private string pass;
        private string name;
        private SmtpClient smtp;

        public MailManager()
        {
            this.address = "KovrovKuBot@gmail.com";
            this.pass = "KovrovKuBot1";
            this.name = "Бот Карьероуправления";
            this.smtp = new SmtpClient("smtp.gmail.com", 587);
            this.smtp.Credentials = new NetworkCredential(address, pass);
            this.smtp.EnableSsl = true;
        }

        public void SendMail(string addressTo, Incident incident)
        {
         
            MailAddress from = new MailAddress(address, name);
            MailAddress to = new MailAddress(addressTo);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Новый инцидент";
            m.Body =
                String.Format(
                    "<h2>В смену {0} от {1} произошел инцидент!</h2>" + "<p>Сведения о инциденте:</p>" +
                    "<p>Время:{2}</p>" + "<p>Датчик:{3}</p>",  incident.Shift.Number, incident.Shift.Date.ToShortDateString(), incident.Time.ToShortTimeString(),
                    incident.Sensor.Name);
           
            m.IsBodyHtml = true;
            this.smtp.Send(m);
        }
    }
}