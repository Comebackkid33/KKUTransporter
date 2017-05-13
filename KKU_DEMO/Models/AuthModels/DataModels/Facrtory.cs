using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KKU_DEMO.Models.DataModels
{
    public class Factory
    {
        public int Id { get; set; }
        [Display(Name = "Подразделение:")]
        public string Name { get; set; }

        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<Sensor> Sensors { get; set; }
    }
}