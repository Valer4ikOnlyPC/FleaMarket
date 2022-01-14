using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string FirstPhoto { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public bool State { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }
}
