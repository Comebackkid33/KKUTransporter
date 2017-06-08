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
    public class FactoryRepository : IRepository<Factory>
    {
        private bool disposed = false;
        private KKUContext db;

        public FactoryRepository()
        {
            this.db = new KKUContext();
        }

        public IEnumerable<Factory> GetList()
        {
            return db.Factory;
        }

        public Factory Get(int id)
        {
            return db.Factory.Find(id);
        }


        public void Create(Factory item)
        {
            db.Factory.Add(item);
          
        }

        public void Update(Factory item)
        {
            db.Entry(item).State = EntityState.Modified;
           
        }

        public void Delete(int id)
        {
            Factory factory = db.Factory.Find(id);
            if (factory != null)
                db.Factory.Remove(factory);
            
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