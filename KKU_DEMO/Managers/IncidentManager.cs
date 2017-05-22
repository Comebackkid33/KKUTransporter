using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;

namespace KKU_DEMO.Managers
{
    public class IncidentManager
    {
        private MailManager MailManager;
        private TelegramManager TelegramManager;
        private KKUContext db;
   
        public IncidentManager()
        {
            db = new KKUContext();
            MailManager = new MailManager();
            TelegramManager = new TelegramManager();
        }

        public List<Incident> GetAllIncidents()
        {
           
            return db.Incident.OrderBy(u => u.Time).ToList();
        }

        public List<Incident> GetIncidentsByUser(string id)
        {
          
            return db.Incident.Where(u => u.Shift.UserId == id).OrderBy(u => u.Time).ToList();
        }
        /// <summary>
        /// Возвращает открытый инцидент в выбранную смену на выбранном датчике
        /// </summary>
        /// <param name="shiftId"></param>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public Incident GetIncident(int shiftId, int sensorId)
        {
            
            return db.Incident.FirstOrDefault(u => u.ShiftId == shiftId && u.SensorId == sensorId && u.State == StateEnum.OPENED.ToString());
        }
        public Incident GetIncident(int id)
        {
            
            return db.Incident.Find(id);
        }
        public void UpdateIncident(Incident inc)
        {
         

            var incident = db.Incident.Find(inc.Id);
            incident.CauseId = inc.CauseId;
            incident.Cause = db.Cause.Find(inc.CauseId);
            incident.StateEnum = StateEnum.CLOSED;
            db.Entry(incident).State = EntityState.Modified;
            db.SaveChanges();
            
        }

        public void AddIncident(int shiftId, int sensorId)
        {
            
            
                var shift = db.Shift.Find(shiftId);
                var sensor = db.Sensor.Find(sensorId);
                var incident = new Incident
                {
                    ShiftId = shift.Id,
                    Shift = shift,
                    StateEnum = StateEnum.OPENED,
                    Time = DateTime.Now,
                    SensorId = sensor.Id,
                    Sensor = sensor
                };

                Notify(incident);
                db.Incident.Add(incident);
                db.SaveChanges();
            
        }

        public List<Incident> GetByDay(DateTime day)
        {
            var incidents = GetAllIncidents();
            var listOfIncidents = new List<Incident>();
            foreach (var i in incidents)
            {
                if (i.Time.Date == day.Date)
                {
                    listOfIncidents.Add(i);
                }
            }
            return listOfIncidents;
        }
        public void Notify(Incident incident)
        {
            MailManager.SendMail("kurganovk@gmail.com", incident);
            TelegramManager.NotifyBot();
    

            }
    }
}