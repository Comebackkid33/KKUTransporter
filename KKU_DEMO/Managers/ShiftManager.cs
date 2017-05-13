using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;
using KKU_DEMO.Models.AuthModels;
using KKU_DEMO.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace KKU_DEMO.Managers
{
    public class ShiftManager 
    {
        private KKUContext db;
        private RoleManager<IdentityRole> RoleManager;
        private UserManager UserManager;
        private FactoryManager FactoryManager;

        public ShiftManager(HttpContextBase httpContext)
        {
            db = new KKUContext();

            RoleManager =  new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            UserManager = new UserManager(new UserStore<User>(db));
            FactoryManager = new FactoryManager();

            
        }

        public List<Shift> GetAll(int id)
        {
            throw new NotImplementedException();
        }
        public List<Shift> GetShifts(ShiftSearchModel shiftSearchModel)
        {
            List<Shift> shifts = new List<Shift>();
            DateTime defTime = new DateTime();

            var query = db.Shift.AsQueryable();

            if (shiftSearchModel.FactoryId != 0)
            {
                query = query.Where(c => c.FactoryId == shiftSearchModel.FactoryId);
            }

            if (shiftSearchModel.Date != defTime)
            {
                query = query.Where(c => c.Date == shiftSearchModel.Date);
            }
            if (shiftSearchModel.Number != 0)
            {
                query = query.Where(c => c.Number == shiftSearchModel.Number);
            }
            if (shiftSearchModel.UserId != "0" )
            {
                query = query.Where(c => c.UserId == shiftSearchModel.UserId);
            }

            shifts = query.ToList();

         

            if (shifts.Count != 0)
            foreach (var sh in shifts)
            {
                if (sh.StateEnum == StateEnum.INPROCESS)
                {
                    sh.TotalShiftWeight = getTotalWeight(sh);
                }
            }

            return shifts.OrderBy(x => x.Date).ThenBy(x => x.FactoryId).ThenBy(x => x.Number).ToList();
        }
        
        public Shift GetById(int id)
        {
            var shift = db.Shift.Find(id);
            if (shift != null)
            {
                if (shift.StateEnum == StateEnum.INPROCESS)
                {
                    shift.TotalShiftWeight = getTotalWeight(shift);

                }
                
            }
            return shift;
        }

        public void Create(Shift entity)
        {
            throw new NotImplementedException();
        }

        public ShiftCreateModel GetShiftCreateModel()
        {
            var userIds = RoleManager.FindByName("Master").Users.Select(e => e.UserId).ToList();
            var masterlist = UserManager.Users.Where(e => userIds.Contains(e.Id)).ToList();
            var factoryList = FactoryManager.GetAll();
            return new ShiftCreateModel(masterlist,factoryList);

        }
        public ShiftCreateModel GetShiftCreateModel(int id)
        {
            var userIds = RoleManager.FindByName("Master").Users.Select(e => e.UserId).ToList();
            var masterlist = UserManager.Users.Where(e => userIds.Contains(e.Id)).ToList();
            var factoryList = FactoryManager.GetAll();

            var shift = db.Shift.Find(id);
            return new ShiftCreateModel(masterlist, factoryList,shift);


        }
        public ShiftSearchModel GetShiftSearchModel()
        {
            var userIds = RoleManager.FindByName("Master").Users.Select(e => e.UserId).ToList();
            var masterlist = UserManager.Users.Where(e => userIds.Contains(e.Id)).ToList();
            var factoryList = FactoryManager.GetAll();
            return new ShiftSearchModel(masterlist, factoryList);

        }
        public void Create(ShiftCreateModel shiftCreateModel)
        {

            Shift shift = new Shift(shiftCreateModel);

            shift.Factory = db.Factory.Where(b => b.Id == shift.FactoryId).Select(b => b).FirstOrDefault();
            shift.User = UserManager.FindById(shift.UserId);
            

            db.Entry(shift).State = EntityState.Added;
            db.SaveChanges();


        }
      

        public void Delete(int id)
        {
            var shift = db.Shift.Find(id);
            db.Shift.Remove(shift);
            db.SaveChanges();
        }

        public void Update(ShiftCreateModel shiftCreateModel)
        {
            Shift shift = db.Shift.Find(shiftCreateModel.Id);

            shift.Factory = db.Factory.Where(b => b.Id == shift.FactoryId).Select(b => b).FirstOrDefault();
            shift.User = UserManager.FindById(shift.UserId);
            shift.DownTime = shiftCreateModel.DownTime;
            shift.ProductionPct = shiftCreateModel.ProductionPct;
            shift.TotalShiftWeight = shiftCreateModel.TotalShiftWeight;
            
            db.SaveChanges();
        }

        public bool CheckExistense(ShiftCreateModel shiftCreateModel)
        {
            var shift = db.Shift.FirstOrDefault(s => s.Date == shiftCreateModel.Date && s.Number == shiftCreateModel.Number && s.FactoryId == shiftCreateModel.FactoryId);
            return shift == null;
        }

        public double getTotalWeight(Shift shift)
        {
            var totalSensor = db.Sensor.FirstOrDefault(s => s.FactoryId == shift.FactoryId && s.Name == "ВХОД");
            if (totalSensor != null)
            {
               return totalSensor.TotalWeight - shift.TotalShiftWeight;
            }
            else
            {
                return shift.TotalShiftWeight;
            }
        }
    }
}