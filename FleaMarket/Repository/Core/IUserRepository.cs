using Domain.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Core
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        Guid Create(User item, string password);
        bool Verification(User item, string password);
        User Update(Guid id, User item);
        void Delete(Guid id);
    }
}
