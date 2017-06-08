using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;
using KKU_DEMO.Models.ViewModels;

namespace KKU_DEMO.Repositories
{
    class ShiftRepository: IRepository<Shift>
    {
        private bool disposed = false;
        private KKUContext db;

        public ShiftRepository()
        {
            this.db = new KKUContext();
        }

        public IEnumerable<Shift> GetList()
        {
            return db.Shift;
        }

        public Shift Get(int id)
        {
            return db.Shift.Find(id);
        }

       
        public void Create(Shift item)
        {

            db.Shift.Add(item);
          
        }

        public void Update(Shift item)
        {
            db.Entry(item).State = EntityState.Modified;
          
        }

        public void Delete(int id)
        {
            Shift shift = db.Shift.Find(id);
            if (shift != null)
                db.Shift.Remove(shift);
          
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
