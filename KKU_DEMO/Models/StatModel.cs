using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Antlr.Runtime.Misc;
using KKU_DEMO.Models.DataModels;

namespace KKU_DEMO.Models
{
    public class StatModel
    {
        [Display(Name = "Дата:")]
        public string Date { get; set; }
        [Display(Name = "Общая выработка:")]
        public double TotalWeight { get; set; }
        [Display(Name = "Время простоя:")]
        public int DownTime { get; set; }
        [Display(Name = "Процент выхода:")]
        public double ProductionPct { get; set; }
     
       
        public List<ExelRow> ExelTable { get; set; }

        public StatModel()
        {
            Date = null;
            TotalWeight = 0;
            DownTime = 0;
            ProductionPct = 0;
            ExelTable = new List<ExelRow>();

        }

      


    }

    public class ExelRow
    {
        public string Date { get; set; }
        public double TotalDayWeight { get; set; }
        public double TotalProduction { get; set; }
        public int IncidentCount { get; set; }
    }
}
