using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.Deal;

namespace Domain.Dto
{
    public class DealDto
    {
        public Guid DealId { get; set; }
        public Guid UserMaster { get; set; }
        public string UserMasterName { get; set; }
        public Guid ProductMaster { get; set; }
        public string ProductMasterName { get; set; }
        public Guid UserRecipient { get; set; }
        public string UserRecipientName { get; set; }
        public Guid ProductRecipient { get; set; }
        public string ProductRecipientName { get; set; }
        public enumIsActive IsActive { get; set; }
    }
}
