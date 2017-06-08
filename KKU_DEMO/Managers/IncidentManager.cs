using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;
using KKU_DEMO.Repositories;

namespace KKU_DEMO.Managers
{
    public class IncidentManager
    {
        KKUContext db = new KKUContext();
        private MailManager MailManager;
        private TelegramManager TelegramManager;
        private IRepository<Incident> IncidentRepo;
   
        public IncidentManager()
        {
            IncidentRepo = new IncidentRepository();
            MailManager = new MailManager();
            TelegramManager = new TelegramManager();
        }

        public List<Incident> GetAllIncidents(int factoryId=0)
        {
           var intedentList = new  List<Incident>();
            if (factoryId == 0)
            {
                intedentList = IncidentRepo.GetList().ToList();

            }
            else
            {
                intedentList = IncidentRepo.GetList().Where(s => s.Shift.FactoryId == factoryId).ToList();
            }
            return intedentList.OrderBy(u => u.Time).ToList();
        }

        public List<Incident> GetIncidentsByUser(string id)
        {
          
            return IncidentRepo.GetList().Where(u => u.Shift.UserId == id).OrderBy(u => u.Time).ToList();
        }
        /// <summary>
        /// Возвращает открытый инцидент в выбранную смену на выбранном датчике
        /// </summary>
        /// <param name="shiftId"></param>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public Incident GetIncident(int shiftId, int sensorId)
        {
            
            return IncidentRepo.GetList().FirstOrDefault(u => u.ShiftId == shiftId && u.SensorId == sensorId && u.State == StateEnum.OPENED.ToString());
        }
        public Incident GetIncident(int id)
        {
            
            return IncidentRepo.Get(id);
        }
        public void UpdateIncident(Incident inc)
        {

            var cause = db.Cause.Find(inc.CauseId);
            var incident = IncidentRepo.Get(inc.Id);
            incident.CauseId = inc.CauseId;
            incident.Cause = cause;
            incident.StateEnum = StateEnum.CLOSED;

            IncidentRepo.Update(incident);
            IncidentRepo.Save();
            
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

        public List<Incident> GetByDay(DateTime day,int factoryId)
        {
            var incidents = GetAllIncidents(factoryId);
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
            TelegramManager.NotifyBot(incident);
    

            }
    }
}