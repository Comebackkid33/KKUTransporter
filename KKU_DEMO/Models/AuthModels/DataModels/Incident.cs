using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KKU_DEMO.Models
{
    public class Incident
    {
        public int Id { get; set; }
        [Display(Name = "Состояние:")]
        public string State { get; set; }
        [Display(Name = "Дата создания:")]
        public DateTime Time { get; set; }
        [Display(Name = "Примечание:")]
        public string Note { get; set; }
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
       
        public int? ShiftId { get; set; }
        public virtual Shift Shift { get; set; }
        public int? SensorId { get; set; }
        public virtual Sensor Sensor { get; set; }
        [Display(Name = "Причина:")]
        public int? CauseId { get; set; }
        public virtual Cause Cause { get; set; }

    }
}