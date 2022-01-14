using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class User
    {
        public int UserId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string VkAddress { get; set; }
        public int Rating { get; set; }
        public string City { get; set; }
        private string Password { get; set; }
        public bool CheckedPassword(string password)
        {
            if(Password==password)
                return true;
            return false;
        }

    }
}
