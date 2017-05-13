using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using KKU_DEMO.Models.DataModels;
using KKU_DEMO;

namespace KKU_DEMO.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите название устройства")]
        [Display(Name = "Датчик:")]
        public string Name { get; set; }
        [Display(Name = "Уникальный ключ:")]
        public string Token { get; set; }
        [Display(Name = "Дата:")]
        public string Date { get; set; }
        [Display(Name = "Текущие показания:")]
        public string CurWeight { get; set; }
        [Display(Name = "Всего:")]
        public double TotalWeight { get; set; }
        [Display(Name = "Время простоя:")]
        public int DownTime { get; set; }
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

                return StateEnum.STOP;
            }
            set
            {
                this.State = value.ToString();
            }
        }


        public int NoLoadCount { get; set; }

    
        public int? FactoryId { get; set; }
        public virtual Factory Factory { get; set; }

        public override string ToString()
        {
            return
                String.Format(
                    "Время: {0} Подразделение: {1}| Устройство: {2}| Текущие показания: {3}| Всего: {4}| Состояние: {5} ",
                    Date, Factory.Name, Name, CurWeight, TotalWeight, State);
        }

    }
}