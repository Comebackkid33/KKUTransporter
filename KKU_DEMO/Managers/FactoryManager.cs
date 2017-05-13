using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models.DataModels;

namespace KKU_DEMO.Managers
{
    public class FactoryManager :IManager<Factory>
    {
        private KKUContext db;

        public FactoryManager()
        {
            db = new KKUContext();
        }

        public List<Factory> GetAll()
        {
            return db.Factory.ToList();
        }

        public Factory GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Create(Factory entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Factory entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Factory entity)
        {
            throw new NotImplementedException();
        }
    }
}