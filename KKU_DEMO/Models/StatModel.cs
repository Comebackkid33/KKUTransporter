using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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
     

       
    }
}