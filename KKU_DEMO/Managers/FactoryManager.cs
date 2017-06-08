using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models.DataModels;
using KKU_DEMO.Repositories;

namespace KKU_DEMO.Managers
{
    public class FactoryManager 
    {
        private IRepository<Factory> FactoryRepo;

        public FactoryManager()
        {
            FactoryRepo = new FactoryRepository();
        }

        public List<Factory> GetAll()
        {
            return FactoryRepo.GetList().ToList();
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