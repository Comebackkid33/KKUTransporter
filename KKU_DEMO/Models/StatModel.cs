using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public int? FactoryId { get; set; }
        public Factory Factory { get; set; }
        public List<SelectListItem> FactoryList { get; set; }
        public List<ExelRow> ExelTable { get; set; }
        public List<IncidentRow> IncidentTable { get; set; }

        public StatModel(List<Factory> factoryList)
        {
            Date = null;
            TotalWeight = 0;
            DownTime = 0;
            ProductionPct = 0;
            ExelTable = new List<ExelRow>();
            IncidentTable = new List<IncidentRow>();

            FactoryList = new List<SelectListItem>();
            FactoryList.Add(new SelectListItem() { Text = "---Все подразделения---", Value = "0" });
            foreach (var i in factoryList)
            {
                FactoryList.Add(new SelectListItem() { Text = i.Name.ToString(), Value = i.Id.ToString() });
            }

        }

        public StatModel()
        {
        }


    }

    public class ExelRow
    {
        [Display(Name = "Дата:")]
        public string Date { get; set; }
        public double TotalDayWeight { get; set; }
        public double TotalProduction { get; set; }
        public int IncidentCount { get; set; }
    }

    public class IncidentRow
    {
        public string Cause { get; set; }
        public int IncidentCount { get; set; }
    }
}
