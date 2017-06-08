using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;
using KKU_DEMO.Models.DataModels;

namespace KKU_DEMO.Repositories
{
    public class IncidentRepository : IRepository<Incident>
    {
        private bool disposed = false;
        private KKUContext db;

        public IncidentRepository()
        {
            this.db = new KKUContext();
        }

        public IEnumerable<Incident> GetList()
        {
            return db.Incident;
        }

        public Incident Get(int id)
        {
            return db.Incident.Find(id);
        }


        public void Create(Incident item)
        {
            db.Incident.Add(item);

        }

        public void Update(Incident item)
        {
            db.Entry(item).State = EntityState.Modified;

        }

        public void Delete(int id)
        {
            Incident incident = db.Incident.Find(id);
            if (incident != null)
                db.Incident.Remove(incident);

        }

        public void Save()
        {
            db.SaveChanges();
        }


        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.SaveChanges();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}