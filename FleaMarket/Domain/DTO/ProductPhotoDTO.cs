using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.Product;

namespace Domain.DTO
{
    public class ProductPhotoDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string FirstPhoto { get; set; }
        public string Description { get; set; }
        public int CityId { get; set; }
        public ProductState IsActive { get; set; }
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<string> Image { get; set; }
    }
}
