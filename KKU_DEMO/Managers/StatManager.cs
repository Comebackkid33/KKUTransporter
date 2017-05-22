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

        public StatManager()
        {
            db = new KKUContext();
            ShiftManager = new ShiftManager();
            IncidentManager = new IncidentManager();
        }

        public StatModel ByPeriod(string Start, string End)
        {
            var stat = new StatModel();

            var start = StringToDate(Start);
            var end = StringToDate(End);


            stat.Date = Start + " - " + End;
            stat = SetLists(start, end, stat);
           

            var shifts = ShiftManager.GetByTimeInterval(start, end);
              

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

        public DateTime StringToDate(string date)
        {
            return DateTime.Parse(date + " 00:00:00.000", System.Globalization.CultureInfo.InvariantCulture);
        }
        public StatModel SetLists(DateTime start, DateTime end,StatModel stat)
        {
            while (end.Date != start.Date)
            {
                if (ShiftManager.CheckShiftsOnDay(start))
                {
                    
                    var  row = new ExelRow()
                    {
                        Date = start.ToShortDateString(),
                        TotalDayWeight = ShiftManager.GetShiftTotalWeightByDay(start),
                        TotalProduction = ShiftManager.GetShiftProductionByDay(start),
                        IncidentCount = IncidentManager.GetByDay(start).Count

                    };
                    stat.ExelTable.Add(row);
                }
                start =  start.AddDays(1);
            }
            return stat;
        }
    }
}