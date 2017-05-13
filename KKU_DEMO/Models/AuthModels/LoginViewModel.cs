using System;
using System.ComponentModel.DataAnnotations;

namespace KKU_DEMO.Models.AuthModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
       
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}