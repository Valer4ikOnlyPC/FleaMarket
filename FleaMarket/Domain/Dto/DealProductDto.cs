using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class DealProductDto
    {
        public Guid DealId { get; set; }
        public Guid UserMaster { get; set; }
        public string UserMasterName { get; set; }
        public Product ProductMaster { get; set; }
        public Guid UserRecipient { get; set; }
        public string UserRecipientName { get; set; }
        public Product ProductRecipient { get; set; }
        public DealIsActive IsActive { get; set; }
        public DateTime Date { get; set; }
    }
}
