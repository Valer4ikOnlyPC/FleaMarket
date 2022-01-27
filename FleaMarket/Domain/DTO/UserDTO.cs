using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    public class UserDTO
    {
        public string? Surname { get; set; }
        public string? Name { get; set; }

        [Required(ErrorMessage = "Не указан телефон")]
        public string PhoneNumber { get; set; }
        public string? VkAddress { get; set; }
        public int CityId { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
