using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        Guid Create(User item, string password);
        string Verification(string phoneNumber);
        User Update(Guid id, User item);
        void Delete(Guid id);
    }
}
