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
using WebGrease.Css.Ast.Selectors;

namespace KKU_DEMO.Managers
{
    public class ShiftManager : IManager<Shift>
    {
        private KKUContext db;
        private RoleManager<IdentityRole> RoleManager;
        private UserManager UserManager;
        private FactoryManager FactoryManager;
        private SensorManager SensorManager;

        public ShiftManager(HttpContextBase httpContext)
        {
            db = new KKUContext();

            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
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

        public ShiftManager(SensorManager sensorManager)
        {
            db = new KKUContext();


            FactoryManager = new FactoryManager();
            SensorManager = sensorManager;
        }

        public List<Shift> GetAll(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Возвращает список смен, попараметрам поиска
        /// </summary>
        /// <param name="shiftSearchModel">Модель поиска смены</param>
        /// <returns></returns>
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
            if (shiftSearchModel.UserId != "0")
            {
                query = query.Where(c => c.UserId == shiftSearchModel.UserId);
            }

            shifts = query.ToList();


            if (shifts.Count != 0)
                foreach (var sh in shifts)
                {
                    if (sh.StateEnum == StateEnum.INPROCESS)
                    {
                        sh.TotalShiftWeight = GetTotalWeight(sh);
                    }
                }

            return shifts.OrderBy(x => x.Date).ThenBy(x => x.FactoryId).ThenBy(x => x.Number).ToList();
        }

        public List<Shift> GetAll()
        {
            return db.Shift.ToList();
        }

        public Shift GetById(int id)
        {
            var shift = db.Shift.Find(id);
            if (shift != null)
            {
                if (shift.StateEnum == StateEnum.INPROCESS)
                {
                    shift.TotalShiftWeight = GetTotalWeight(shift);
                    shift.ProductionPct = ProductionPct(shift);
                }
            }
            return shift;
        }

        /// <summary>
        /// Возвращает все смены подразделения (определенного состояния)
        /// </summary>
        /// <param name="id">ID подразделения</param>
        /// <param name="state">Состояние смены</param>
        /// <returns></returns>
        public List<Shift> GetByFactoryId(int? id, string state = null)
        {
            var query = db.Shift.AsQueryable();
            query = db.Shift.Where(s => s.Factory.Id == id);

            if (state != null)
            {
                query = db.Shift.Where(s => s.State == state);
            }

            var shifts = query.OrderBy(x => x.Date).ThenBy(x => x.Number).ToList();
            return shifts;
        }

        public void Create(Shift entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Shift entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Shift entity)
        {
            throw new NotImplementedException();
        }

        public ShiftCreateModel GetShiftCreateModel()
        {
            var userIds = RoleManager.FindByName("Master").Users.Select(e => e.UserId).ToList();
            var masterlist = UserManager.Users.Where(e => userIds.Contains(e.Id)).ToList();
            var factoryList = FactoryManager.GetAll();
            return new ShiftCreateModel(masterlist, factoryList);
        }

        public ShiftCreateModel GetShiftCreateModel(int id)
        {
            var userIds = RoleManager.FindByName("Master").Users.Select(e => e.UserId).ToList();
            var masterlist = UserManager.Users.Where(e => userIds.Contains(e.Id)).ToList();
            var factoryList = FactoryManager.GetAll();

            var shift = db.Shift.Find(id);
            return new ShiftCreateModel(masterlist, factoryList, shift);
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

            if (CheckExistense(shiftCreateModel))
            {
                db.SaveChanges();
            }
            else
            {
                throw new Exception("Смена уже существует или не была изменена");
            }
        }

        public bool CheckExistense(ShiftCreateModel shiftCreateModel)
        {
            var shift = db.Shift.FirstOrDefault(s => s.Date == shiftCreateModel.Date &&
                                                     s.Number == shiftCreateModel.Number &&
                                                     s.FactoryId == shiftCreateModel.FactoryId);
            if (shiftCreateModel.Id != null && shift != null)
            {
                if (shiftCreateModel.Id == shift.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return shift == null;
            }
        }

        public double GetTotalWeight(Shift shift)
        {
            var totalSensor = SensorManager.GetByFactoryId(shift.FactoryId, "Вход");
            if (totalSensor != null)
            {
                return totalSensor.TotalWeight - shift.TotalShiftWeight;
            }
            else
            {
                return shift.TotalShiftWeight;
            }
        }

        public double ProductionPct(Shift shift)
        {
            var wasteSensor = SensorManager.GetByFactoryId(shift.FactoryId, "Отсев");
            if (wasteSensor != null)
            {
                return (1 - (wasteSensor.TotalWeight - shift.ProductionPct) / shift.TotalShiftWeight) * 100;
            }
            else
            {
                return shift.TotalShiftWeight;
            }
        }
        /// <summary>
        /// Проверяет, есть ли смены в выбранный день
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool CheckShiftsOnDay(DateTime day, int factoryId)
        {
            Shift shifts = null;
            if (factoryId == 0)
            { 
                 shifts = db.Shift.FirstOrDefault(s => s.Date == day && s.State == StateEnum.CLOSED.ToString());
            }
            else
            {
                shifts = db.Shift.FirstOrDefault(s => s.Date == day && s.State == StateEnum.CLOSED.ToString() && s.FactoryId==factoryId);
            }
            return shifts != null;
        }
        /// <summary>
        /// Возвращает общую выработку за день по всем подразделениям
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public double GetShiftTotalWeightByDay(DateTime day, int factoryId)
        {
            double totalweightByDay = 0;

            var shifts = new List<Shift>();

            if (factoryId == 0)
            {
                shifts = db.Shift.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString()).ToList();

            }
            else
            {
                shifts = db.Shift.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString() && s.FactoryId==factoryId).ToList();
            }
            foreach (var s in shifts)
                {
                    totalweightByDay += s.TotalShiftWeight;
                }
                return totalweightByDay;
            
           
            
        }

        /// <summary>
        /// Возвращает средний коофициент выхода за день по всем подразделениям
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public double GetShiftProductionByDay(DateTime day,int factoryId)
        {
            double ProductionByDay = 0;
            var shifts = new List<Shift>();

            if (factoryId == 0)
            {
                shifts = db.Shift.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString()).ToList();

            }
            else
            {
                shifts = db.Shift.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString() && s.FactoryId == factoryId).ToList();
            }
            foreach (var s in shifts)
            {
                ProductionByDay += s.ProductionPct;
            }
            return (shifts.Count == 0) ? 0 : ProductionByDay / shifts.Count;
        }

        /// <summary>
        /// Список всех смен в интервал времени, во всех подразделениях
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Shift> GetByTimeInterval(DateTime start, DateTime end, int factoryId)
        {
            var shifts = new List<Shift>();
            if (factoryId == 0)
            {
                shifts =
                    db.Shift.Where(
                        c =>
                            c.Date.CompareTo(start) >= 0 && c.Date.CompareTo(end) <= 0 &&
                            c.State == StateEnum.CLOSED.ToString())
                        .Select(c => c)
                        .OrderBy(c => c.Date)
                        .ToList();

            }
            else
            {
                shifts =
                db.Shift.Where(
                        c =>
                            c.Date.CompareTo(start) >= 0 && c.Date.CompareTo(end) <= 0 &&
                            c.State == StateEnum.CLOSED.ToString() && c.FactoryId==factoryId)
                    .Select(c => c)
                    .OrderBy(c => c.Date)
                    .ToList();
            }
            
            return shifts;
        }

        /// <summary>
        /// Переключает смену если есть следующая, если нет, заканчивает текущую смену
        /// </summary>
        /// <param name="id">ID подразделения</param>
        public void ChangeShift(int id)
        {
            //список всех смен на подразделении
            var shifts = GetByFactoryId(id);
            //входной датчик
            var totalSensor = SensorManager.GetByFactoryId(id, "Вход");
            //датчик отсева
            var wasteSensor = SensorManager.GetByFactoryId(id, "Отсев");
            bool flag = false;

            if (shifts != null && totalSensor != null)
            {
                for (int i = 0; i < shifts.Count() - 1; i++)

                {
                    if (shifts[i].StateEnum == StateEnum.INPROCESS)
                    {
                        shifts[i].StateEnum = StateEnum.CLOSED;
                        shifts[i].TotalShiftWeight = totalSensor.TotalWeight - shifts[i].TotalShiftWeight;
                        shifts[i].ProductionPct = (1 -
                                                   (wasteSensor.TotalWeight - shifts[i].ProductionPct) /
                                                   shifts[i].TotalShiftWeight) * 100;


                        flag = true;
                        if (shifts[i + 1].Date == shifts[i].Date && shifts[i + 1].Number == shifts[i].Number + 1 ||
                            shifts[i].Date.AddDays(1) == shifts[i + 1].Date && shifts[i + 1].Number == 1)
                        {
                            shifts[i + 1].StateEnum = StateEnum.INPROCESS;
                            shifts[i + 1].TotalShiftWeight = totalSensor.TotalWeight;
                            shifts[i + 1].ProductionPct = wasteSensor.TotalWeight;
                        }

                        break;
                    }
                }
                if (flag == false)
                {
                    for (int i = 0; i < shifts.Count(); i++)

                    {
                        if (shifts[i].StateEnum == StateEnum.ASSIGNED)
                        {
                            shifts[i].StateEnum = StateEnum.INPROCESS;
                            shifts[i].TotalShiftWeight = totalSensor.TotalWeight;
                            shifts[i].ProductionPct = wasteSensor.TotalWeight;
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