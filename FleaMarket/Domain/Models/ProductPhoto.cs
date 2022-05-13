using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ProductPhoto
    {
        public Guid PhotoId { get; set; }
        public string Link { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
