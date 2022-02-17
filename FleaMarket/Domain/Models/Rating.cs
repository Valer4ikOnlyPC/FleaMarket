using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Rating
    {
        public Guid RatingId { get; set; }
        public int Grade { get; set; }
        public Guid UserMasterId { get; set; }
        public Guid UserRecipientId { get; set; }
        public Guid DealId { get; set; }
    }
}
