using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using KKU_DEMO.Models.AuthModels;
using KKU_DEMO.Models.DataModels;
using KKU_DEMO.Models.ViewModels;

namespace KKU_DEMO.Models
{
    public class Shift
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
            set
            {
                this.State = value.ToString();
            }
        }

        public int? FactoryId { get; set; }
        public virtual Factory Factory { get; set; }
        [Display(Name = "Мастер:")]
        public String UserId { get; set; }
        [Display(Name = "Мастер:")]
        public virtual User User { get; set; }
    
        public virtual ICollection<Incident> Incidents { get; set; }

        public Shift(ShiftCreateModel shiftCreateModel )
        {
            Date = shiftCreateModel.Date;
            FactoryId = shiftCreateModel.FactoryId;
            Number = shiftCreateModel.Number;
            UserId = shiftCreateModel.UserId;
            StateEnum = StateEnum.ASSIGNED;
        }
        public Shift()
        {
            
        }

    }
   
}