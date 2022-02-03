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
        public Task<Guid> Create(UserPassword userPassword);
        public Task<UserPassword> GetById(Guid passwordId);
        public Task<UserPassword> GetByUserId(Guid userID);
    }
}
