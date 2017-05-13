using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.Models.AuthModels;
using KKU_DEMO.Models.DataModels;

namespace KKU_DEMO.Models.ViewModels
{
    public class ShiftCreateModel

    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата:")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Номер:")]
        public int Number { get; set; }

        [Display(Name = "Общая выработка:")]
        public double TotalShiftWeight { get; set; }

        [Display(Name = "Время простоя:")]
        public int DownTime { get; set; }

        [Display(Name = "Процент выхода:")]
        public double ProductionPct { get; set; }

        [Display(Name = "Состояние:")]
        public string State { get; set; }

        [NotMapped]
        public StateEnum StateEnum
        {
            get
            {
                StateEnum s;

                if (Enum.TryParse(State, out s))
                {
                    return s;
                }

                return StateEnum.CLOSED;
            }
            set { this.State = value.ToString(); }
        }

        public int? FactoryId { get; set; }
        public virtual Factory Factory { get; set; }

        [Display(Name = "Мастер:")]
        public String UserId { get; set; }

        [Display(Name = "Мастер:")]
        public virtual User User { get; set; }

        public virtual ICollection<Incident> Incidents { get; set; }

        [NotMapped]
        public List<SelectListItem> MasterList { get; set; }

        [NotMapped]
        public List<SelectListItem> FactoryList { get; set; }

        [NotMapped]
        public List<SelectListItem> NumberList { get; set; }

        public ShiftCreateModel(List<User> masterList, List<Factory> factoryList)
        {
            MasterList = new List<SelectListItem>();
            foreach (var i in masterList)
            {
                MasterList.Add(new SelectListItem() {Text = i.Name.ToString(), Value = i.Id.ToString()});
            }

            FactoryList = new List<SelectListItem>();
            foreach (var i in factoryList)
            {
                FactoryList.Add(new SelectListItem() {Text = i.Name.ToString(), Value = i.Id.ToString()});
            }
            NumberList = new List<SelectListItem>();
            for (int i = 1; i < 4; i++)

            {
                NumberList.Add(new SelectListItem() {Text = i.ToString(), Value = i.ToString()});
            }
        }

        public ShiftCreateModel(List<User> masterList, List<Factory> factoryList, Shift shift)
        {
            MasterList = new List<SelectListItem>();
            foreach (var i in masterList)
            {
                MasterList.Add(new SelectListItem() {Text = i.Name.ToString(), Value = i.Id.ToString()});
            }

            FactoryList = new List<SelectListItem>();
            foreach (var i in factoryList)
            {
                FactoryList.Add(new SelectListItem() {Text = i.Name.ToString(), Value = i.Id.ToString()});
            }
            NumberList = new List<SelectListItem>();
            for (int i = 1; i < 4; i++)

            {
                NumberList.Add(new SelectListItem() {Text = i.ToString(), Value = i.ToString()});
            }

            Date = shift.Date;
            FactoryId = shift.FactoryId;
            Number = shift.Number;
            UserId = shift.UserId;
            StateEnum = StateEnum.ASSIGNED;
            TotalShiftWeight = shift.TotalShiftWeight;
            DownTime = shift.DownTime;
            ProductionPct = shift.ProductionPct;


        }

        public ShiftCreateModel()
        {
        }
    }
}