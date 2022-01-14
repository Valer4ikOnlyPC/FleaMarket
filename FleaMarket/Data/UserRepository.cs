using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UserRepository: IUserRepository
    {
        private CoreContext db;
        public UserRepository()
        {
            this.db = new CoreContext();
        }
        public IEnumerable<User> GetUserList()
        {
            return db.Users.ToArray();
        }
        public User GetUser(int id)
        {
            return db.Users.ToArray().Where(u => u.UserId == id).First();
        }
        public void Create(User item)
        {
            db.Users.Add(item);
        }
        public void Update(int id, User item)
        {
            User selectedUser = db.Users.ToArray().Where(u => u.UserId == id).First();
            selectedUser.Surname = item.Surname;
            selectedUser.Name = item.Name;
            selectedUser.VkAddress = item.VkAddress;
            selectedUser.Rating = item.Rating;
            selectedUser.City = item.City;
            Save();
        }
        //void UpdatePassword(User item);
        public bool ChekPassword(User item, string password)
        {
            return item.CheckedPassword(password);
        }
        public void Delete(int id)
        {
            db.Users.Remove(db.Users.ToArray().Where(u => u.UserId == id).First());
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
