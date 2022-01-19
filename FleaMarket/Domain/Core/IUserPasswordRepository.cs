using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IUserPasswordRepository
    {
        public Guid Create(string password);
        public string GetById(Guid passwordId);
    }
}
