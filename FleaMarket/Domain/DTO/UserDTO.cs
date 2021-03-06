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
        [Required]
        [StringLength(18, MinimumLength = 3)]
        public string? Surname { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Не указан телефон")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Url]
        public string? VkAddress { get; set; }
        [Required]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
