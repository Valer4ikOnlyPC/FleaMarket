using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Не указан телефон")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
