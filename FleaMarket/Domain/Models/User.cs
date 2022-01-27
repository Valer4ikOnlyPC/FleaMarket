using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? VkAddress { get; set; }
        public float Rating { get; set; } = 0;
        public int CityId { get; set; }
        public bool IsDelete { get; set; }
        public Guid PasswordId { get; set; }
    }
}
