using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class SearchDto
    {
        [Required]
        [StringLength(30)]
        public string? Search { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int CityId { get; set; }
    }
}
