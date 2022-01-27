using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.Product;

namespace Domain.DTO
{
    public class ProductPhotoDTO
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string FirstPhoto { get; set; }
        public string Description { get; set; }
        public int CityId { get; set; }
        public enumIsActive IsActive { get; set; }
        public int CategoryId { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<string> Image { get; set; }
    }
}
