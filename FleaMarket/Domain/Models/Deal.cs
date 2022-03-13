using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Deal
    {
        public Guid DealId { get; set; }
        public Guid UserMaster { get; set; }
        public Guid ProductMaster { get; set; }
        public Guid UserRecipient { get; set; }
        public Guid ProductRecipient { get; set; }
        public DealState IsActive { get; set; }
        public DateTime Date { get; set; }
    }
}
