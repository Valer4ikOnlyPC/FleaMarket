using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ErrorModel: Exception
    {
        public int Number { get; set; } = 500;
        public bool ShowRequestId => !string.IsNullOrEmpty(Message);
        public string Message { get; set; }
        public ErrorModel(int number, string message)
        {
            Number = number;
            Message = message;
        }
    }
}
