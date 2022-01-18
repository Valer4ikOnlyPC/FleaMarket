using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string FirstPhoto { get; set; }
        public string Description { get; set; }
        public int CityId { get; set; }
        public enumIsActive IsActive { get; set; }
        public int CategoryId { get; set; }
        public Guid UserId { get; set; }

        public enum enumIsActive : int
        {
            Closed,
            InDeal,
            Active
        }
    }
}
