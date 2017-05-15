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
        private SensorManager SensorManager;

        public ShiftManager(HttpContextBase httpContext)
        {
            db = new KKUContext();

            RoleManager =  new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            UserManager = new UserManager(new UserStore<User>(db));
            FactoryManager = new FactoryManager();
            SensorManager = new SensorManager();

        }
        public ShiftManager()
        {
            db = new KKUContext();

            
            FactoryManager = new FactoryManager();
            SensorManager = new SensorManager();


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

        public List<Shift> GetByFactoryId(int id)
        {
            
            var sh = db.Shift.Where(s => s.Factory.Id == id).ToList();
            var shifts = sh.OrderBy(x => x.Date).ThenBy(x => x.Number).ToList();
            return shifts;
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
            shift.Date = shiftCreateModel.Date;
            shift.Number = shiftCreateModel.Number;
            shift.State = shiftCreateModel.State;
            shift.FactoryId = shiftCreateModel.FactoryId;
            shift.Factory = db.Factory.Where(b => b.Id == shift.FactoryId).Select(b => b).FirstOrDefault();
            shift.UserId = shiftCreateModel.UserId;
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

        public void ChangeShift(int id)
        {
            var shifts = GetByFactoryId(id);
            var total = SensorManager.GetByFactoryId(id, "ВХОД");
            bool flag = false;

            if (shifts != null && total != null)
            {
                for (int i = 0; i < shifts.Count() - 1; i++)

                {
                    if (shifts[i].StateEnum == StateEnum.INPROCESS)
                    {
                        shifts[i].StateEnum = StateEnum.CLOSED;
                        shifts[i].TotalShiftWeight = total.TotalWeight - shifts[i].TotalShiftWeight;
                        flag = true;
                        if (shifts[i + 1].Date == shifts[i].Date && shifts[i + 1].Number == shifts[i].Number + 1 ||
                            shifts[i].Date.AddDays(1) == shifts[i + 1].Date && shifts[i + 1].Number == 1)
                        {
                            shifts[i + 1].StateEnum = StateEnum.INPROCESS;
                            shifts[i + 1].TotalShiftWeight = total.TotalWeight;
                        }

                        break;
                    }
                }
                if (flag == false)
                {
                    for (int i = 0; i < shifts.Count() - 1; i++)

                    {
                        if (shifts[i].StateEnum == StateEnum.ASSIGNED)
                        {
                            shifts[i].StateEnum = StateEnum.INPROCESS;
                            shifts[i].TotalShiftWeight = total.TotalWeight;
                            break;
                        }
                    }
                }
                db.SaveChanges();

            }
            else
            {
                throw new Exception("Ошибка поиска смены или датчика");
            }
            
        }
    }
}