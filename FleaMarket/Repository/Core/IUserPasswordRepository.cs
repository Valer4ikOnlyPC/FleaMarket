using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Core
{
    public interface IUserPasswordRepository
    {
        public Guid Create(string password);
        public bool Verification(string hashedPassword, Guid passwordId);
    }
}
