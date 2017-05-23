using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;

namespace KKU_DEMO.Managers
{
    public class StatManager
    {
        private KKUContext db;
        private ShiftManager ShiftManager;
        private IncidentManager IncidentManager;
        private FactoryManager FactoryManager;

        public StatManager()
        {
            db = new KKUContext();
            ShiftManager = new ShiftManager();
            IncidentManager = new IncidentManager();
            FactoryManager = new FactoryManager();
        }

        public StatModel ByPeriod(string Start, string End, int factoryId)
        {
            var factoryList = FactoryManager.GetAll();
            var stat = new StatModel(factoryList);

            var start = StringToDate(Start);
            var end = StringToDate(End);


            stat.Date = Start + " - " + End;

            

            stat = SetLists(start, end, stat,factoryId);
            stat.FactoryId = factoryId;
            stat.Factory = db.Factory.Find(factoryId);

            var shifts = ShiftManager.GetByTimeInterval(start, end, factoryId);
              

            if (shifts.Count != 0)
            {
                foreach (var s in shifts)
                {
                    stat.TotalWeight += s.TotalShiftWeight;
                    stat.DownTime += s.DownTime;
                    stat.ProductionPct += s.ProductionPct;
                   
                }

                stat.ProductionPct /= shifts.Count;
                
                return stat;
            }

            return null;
        }

        public StatModel GetStatModel()
        {
            var factoryList = FactoryManager.GetAll();
            var stat = new StatModel(factoryList);
            return stat;
        }
        public DateTime StringToDate(string date)
        {
            return DateTime.Parse(date + " 00:00:00.000", System.Globalization.CultureInfo.InvariantCulture);
        }
        public StatModel SetLists(DateTime start, DateTime end,StatModel stat,int factoryId)
        {
            while (end.Date != start.Date)
            {
                if (ShiftManager.CheckShiftsOnDay(start, factoryId))
                {
                    
                    var  row = new ExelRow()
                    {
                        Date = start.ToShortDateString(),
                        TotalDayWeight = ShiftManager.GetShiftTotalWeightByDay(start, factoryId),
                        TotalProduction = ShiftManager.GetShiftProductionByDay(start, factoryId),
                        IncidentCount = IncidentManager.GetByDay(start, factoryId).Count

                    };
                    stat.ExelTable.Add(row);

                    foreach (var e in IncidentManager.GetByDay(start,factoryId))
                    {
                        var i = stat.IncidentTable.FirstOrDefault(I => I.Cause == e.Cause.Name);
                        if (i != null)
                        {
                            i.IncidentCount++;
                        }
                        else
                        {
                            stat.IncidentTable.Add(new IncidentRow() {Cause = e.Cause.Name,IncidentCount = 1});
                        }
                    }
                    
                }
                start =  start.AddDays(1);
            }
            

            return stat;
        }


    }
}