using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;
using KKU_DEMO.Models.AuthModels;
using KKU_DEMO.Models.DataModels;
using KKU_DEMO.Models.ViewModels;
using KKU_DEMO.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebGrease.Css.Ast.Selectors;

namespace KKU_DEMO.Managers
{
    public class ShiftManager 
    {
       // private KKUContext db;
        private RoleManager<IdentityRole> RoleManager;
        private UserManager UserManager;
        private FactoryManager FactoryManager;
        private SensorManager SensorManager;

        IRepository<Shift> ShiftRepo;
      

       

        public ShiftManager()
        {
            ShiftRepo = new ShiftRepository();
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new KKUContext()));
            UserManager = new UserManager(new UserStore<User>(new KKUContext()));
            FactoryManager = new FactoryManager();
            SensorManager = new SensorManager();
        }

        public ShiftManager(SensorManager sensorManager)
        {
            ShiftRepo = new ShiftRepository();


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

            var query = ShiftRepo.GetList().AsQueryable();

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
            return ShiftRepo.GetList().ToList();
        }

        public Shift GetById(int id)
        {
            var shift = ShiftRepo.Get(id);
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
            var query = ShiftRepo.GetList().AsQueryable();
            query = query.Where(s => s.Factory.Id == id);

            if (state != null)
            {
                query = query.Where(s => s.State == state);
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

            var shift = ShiftRepo.Get(id);
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

            //Shift shift = new Shift(shiftCreateModel);
            //var factory = FactoryManager.GetAll().Where(b => b.Id == shiftCreateModel.FactoryId).Select(b => b).FirstOrDefault();
            //var user = UserManager.FindById(shiftCreateModel.UserId);

            Shift shift = new Shift()
            {
                Date = shiftCreateModel.Date,
                FactoryId = shiftCreateModel.FactoryId,
                Number = shiftCreateModel.Number,
                UserId = shiftCreateModel.UserId,
                StateEnum = StateEnum.ASSIGNED,
                //Factory = factory,
                //User =user,
        };

           
            ShiftRepo.Create(shift);
            ShiftRepo.Save();
           
        }


        public void Delete(int id)
        {
            ShiftRepo.Delete(id);
            ShiftRepo.Save();
        }

        public void Update(ShiftCreateModel shiftCreateModel)
        {
            //var factories = FactoryManager.GetAll().Where(b => b.Id == shiftCreateModel.FactoryId).Select(b => b).FirstOrDefault();
            //var user = UserManager.FindById(shiftCreateModel.UserId);


            Shift shift = ShiftRepo.Get(shiftCreateModel.Id);
            shift.Date = shiftCreateModel.Date;
            shift.Number = shiftCreateModel.Number;
            shift.State = shiftCreateModel.State;
            shift.FactoryId = shiftCreateModel.FactoryId;
           // shift.Factory = factories;
            shift.UserId = shiftCreateModel.UserId;
            //shift.User = user;
            shift.DownTime = shiftCreateModel.DownTime;
            shift.ProductionPct = shiftCreateModel.ProductionPct;
            shift.TotalShiftWeight = shiftCreateModel.TotalShiftWeight;

            if (CheckExistense(shiftCreateModel))
            {
                ShiftRepo.Update(shift);
                ShiftRepo.Save();
            }
            else
            {
                throw new Exception("Смена уже существует или не была изменена");
            }
        }

        public bool CheckExistense(ShiftCreateModel shiftCreateModel)
        {
            var shifts = ShiftRepo.GetList();
            var shift = shifts.FirstOrDefault(s => s.Date == shiftCreateModel.Date &&
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
            var Shifts = ShiftRepo.GetList();
            if (factoryId == 0)
            { 
                 shifts = Shifts.FirstOrDefault(s => s.Date == day && s.State == StateEnum.CLOSED.ToString());
            }
            else
            {
                shifts = Shifts.FirstOrDefault(s => s.Date == day && s.State == StateEnum.CLOSED.ToString() && s.FactoryId==factoryId);
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

            var shifts = ShiftRepo.GetList(); ;

            if (factoryId == 0)
            {
                shifts = shifts.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString()).ToList();

            }
            else
            {
                shifts = shifts.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString() && s.FactoryId==factoryId).ToList();
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
            var shifts = ShiftRepo.GetList();

            if (factoryId == 0)
            {
                shifts = shifts.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString()).ToList();

            }
            else
            {
                shifts = shifts.Where(s => s.Date == day && s.State == StateEnum.CLOSED.ToString() && s.FactoryId == factoryId).ToList();
            }
            foreach (var s in shifts)
            {
                ProductionByDay += s.ProductionPct;
            }
            return (shifts.Count() == 0) ? 0 : ProductionByDay / shifts.Count();
        }

        /// <summary>
        /// Список всех смен в интервал времени, во всех подразделениях
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Shift> GetByTimeInterval(DateTime start, DateTime end, int factoryId)
        {
            var shifts = ShiftRepo.GetList();
            if (factoryId == 0)
            {
                shifts =
                    shifts.Where(
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
                shifts.Where(
                        c =>
                            c.Date.CompareTo(start) >= 0 && c.Date.CompareTo(end) <= 0 &&
                            c.State == StateEnum.CLOSED.ToString() && c.FactoryId==factoryId)
                    .Select(c => c)
                    .OrderBy(c => c.Date)
                    .ToList();
            }
            
            return shifts.ToList();
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
                        shifts[i].DownTime = totalSensor.DownTime - shifts[i].DownTime;
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
                            shifts[i + 1].DownTime = totalSensor.DownTime;
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
                            shifts[i].DownTime = totalSensor.DownTime;
                            shifts[i].ProductionPct = wasteSensor.TotalWeight;
                            break;
                        }
                    }
                }
                ShiftRepo.Save();
            }
            else
            {
                throw new Exception("Ошибка поиска смены или датчика");
            }

        }
     
       
      
    }
   
}