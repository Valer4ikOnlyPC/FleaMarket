using Domain.Models;
using Domain.Core;
using Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Domain.DTO;

namespace Services.Service
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public Task<Guid> Create(UserDTO item)
        {
            string hashPassword = HashPassword(item.Password);
            User user = new User
            {
                Surname = item.Surname,
                Name = item.Name,
                PhoneNumber = item.PhoneNumber,
                VkAddress = item.VkAddress,
                Rating = 0,
                CityId = item.CityId,
                IsDelete = false,
                PasswordId = new Guid()
            };
            return _userRepository.Create(user, hashPassword);
        }

        public void Delete(Guid id)
        {
            _userRepository.Delete(id);
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public Task<User> GetById(Guid id)
        {
            return _userRepository.GetById(id);
        }
        public Task<User> GetByPhone(string phone)
        {
            return _userRepository.GetByPhone(phone);
        }

        public Task<User> Update(Guid id, User item)
        {
            return _userRepository.Update(id, item);
        }

        public bool Verification(string phoneNumber, string password)
        {

            string hashPassword =  _userRepository.Verification(phoneNumber).Result;
            if(hashPassword=="-1")
                return false;
            return VerifyHashedPassword(hashPassword, password);
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }
        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}
