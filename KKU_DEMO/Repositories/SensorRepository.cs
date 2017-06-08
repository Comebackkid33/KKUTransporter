using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;

namespace KKU_DEMO.Repositories
{
    public class SensorRepository:IRepository<Sensor>
    {
        private bool disposed = false;
        private KKUContext db;

        public SensorRepository()
        {
            this.db = new KKUContext();
        }

        public IEnumerable<Sensor> GetList()
        {
            return db.Sensor;
        }

        public Sensor Get(int id)
        {
            return db.Sensor.Find(id);
        }


        public void Create(Sensor item)
        {

            db.Sensor.Add(item);

        }

        public void Update(Sensor item)
        {
            db.Entry(item).State = EntityState.Modified;

        }

        public void Delete(int id)
        {
            Sensor sensor = db.Sensor.Find(id);
            if (sensor != null)
                db.Sensor.Remove(sensor);

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