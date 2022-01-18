using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Request
    {
        int RequesttId { get; set; }
        string Name { get; set; }
        int CategoryId { get; set; }
        int UserId { get; set; }
    }
}
