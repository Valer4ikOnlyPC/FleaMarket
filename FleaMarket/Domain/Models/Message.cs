using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Message
    {
        public Guid UserId { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
