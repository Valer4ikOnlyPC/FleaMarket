using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Rating
    {
        public Guid RatingId { get; set; }
        [Range(1, 5)]
        public int Grade { get; set; }
        [Required]
        public Guid UserMasterId { get; set; }
        [Required]
        public Guid UserRecipientId { get; set; }
        [Required]
        public Guid DealId { get; set; }
    }
}
