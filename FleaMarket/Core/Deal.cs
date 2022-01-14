using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Deal
    {
        public int DealId { get; set; }
        public int UserMaster { get; set; }
        public int ProducеMaster { get; set; }
        public int UserRecipient { get; set; }
        public int ProductRecipient { get; set; }
        public bool State { get; set; }
    }
}
