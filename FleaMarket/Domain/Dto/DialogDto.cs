using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class DialogDto
    {
        public Guid DialogId { get; set; }
        public Guid User1 { get; set; }
        public string NameUser1 { get; set; }
        public Guid User2 { get; set; }
        public string NameUser2 { get; set; }
        public string Path { get; set; }
        public DateTime Date { get; set; }
    }
}
