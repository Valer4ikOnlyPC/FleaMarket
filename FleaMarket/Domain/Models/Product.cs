using Domain.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        public string? FirstPhoto { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        public int CityId { get; set; }
        public ProductState IsActive { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
    }
}
