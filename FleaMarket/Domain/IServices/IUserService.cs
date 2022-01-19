using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        Guid Create(User item, string password);
        bool Verification(string phoneNumber, string password);
        User Update(Guid id, User item);
        void Delete(Guid id);
    }
}
