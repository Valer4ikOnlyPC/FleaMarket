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
        Task<User> GetById(Guid id);
        Task<User> GetByPhone(string phone);
        Task<Guid> Create(User item, string password);
        Task<string> Verification(string phoneNumber);
        Task<User> Update(Guid id, User item);
        void Delete(Guid id);
    }
}
