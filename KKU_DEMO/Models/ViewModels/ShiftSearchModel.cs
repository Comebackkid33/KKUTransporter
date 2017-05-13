using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.Models.AuthModels;
using KKU_DEMO.Models.DataModels;

namespace KKU_DEMO.Models.ViewModels
{
    public class ShiftSearchModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата:")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Номер:")]
        public int Number { get; set; }

        [Display(Name = "Подразделение:")]
        public int? FactoryId { get; set; }

        public virtual Factory Factory { get; set; }

        [Display(Name = "Мастер:")]
        public String UserId { get; set; }

        public List<SelectListItem> MasterList { get; set; }
        public List<SelectListItem> FactoryList { get; set; }
        public List<SelectListItem> NumberList { get; set; }

        public ShiftSearchModel(List<User> masterList, List<Factory> factoryList)
        {
            MasterList = new List<SelectListItem>();
            MasterList.Add(new SelectListItem() {Text = "---Все Мастера---", Value = "0"});
            foreach (var i in masterList)
            {
                MasterList.Add(new SelectListItem() {Text = i.Name.ToString(), Value = i.Id.ToString()});
            }

            FactoryList = new List<SelectListItem>();
            FactoryList.Add(new SelectListItem() {Text = "---Все подразделения---", Value = "0"});
            foreach (var i in factoryList)
            {
                FactoryList.Add(new SelectListItem() {Text = i.Name.ToString(), Value = i.Id.ToString()});
            }

            NumberList = new List<SelectListItem>();
            NumberList.Add(new SelectListItem() {Text = "---Все ---", Value = "0"});
            for (int i = 1; i < 4; i++)

            {
                NumberList.Add(new SelectListItem() {Text = i.ToString(), Value = i.ToString()});
            }
        }

        public ShiftSearchModel()
        {

        }
    }
}