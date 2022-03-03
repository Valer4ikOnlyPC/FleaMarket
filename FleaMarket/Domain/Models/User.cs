using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        [Required]
        [StringLength(18, MinimumLength = 3)]
        public string Surname { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Url]
        public string? VkAddress { get; set; }
        public float Rating { get; set; } = 0;
        [Required]
        public int CityId { get; set; }
        public bool IsDelete { get; set; }
    }
}
