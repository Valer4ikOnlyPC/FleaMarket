using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserPassword
    {
        public Guid UserPasswordId { get; set; }
        public string Password { get; set; }
    }
}
