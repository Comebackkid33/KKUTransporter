using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KKU_DEMO.DAL;

namespace KKU_DEMO.Managers
{
    interface IManager<T>
    {
        List<T> GetAll();
        T GetById(int id);
        void Create(T entity);
        void Delete(T entity);
        void Update(T entity);

    }
}
