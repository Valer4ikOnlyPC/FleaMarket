using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUserList();
        User GetUser(int id);
        void Create(User item);
        void Update(int id, User item);
        //void UpdatePassword(User item);
        bool ChekPassword(User item, string password);
        void Delete(int id);
        void Save();
    }
}
