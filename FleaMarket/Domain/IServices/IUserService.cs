using Domain.DTO;
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
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(Guid id);
        Task<User> GetByPhone(string phone);
        Task<Guid> Create(UserDTO item);
        Task<bool> Verification(string phoneNumber, string password);
        Task<User> Update(Guid id, User item);
        Task Delete(Guid id);
    }
}
