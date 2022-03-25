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
        private readonly IUserRepository _userRepository;
        private readonly IUserPasswordRepository _userPasswordRepository;

        public UserService(IUserRepository userRepository, IUserPasswordRepository userPasswordRepository)
        {
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
        }

        public async Task<Guid> Create(UserDTO item)
        {
            var user = new User
            {
                Surname = item.Surname,
                Name = item.Name,
                PhoneNumber = item.PhoneNumber,
                VkAddress = item.VkAddress,
                Rating = 0,
                CityId = item.CityId,
                IsDelete = false
            };
            var userId = await _userRepository.Create(user);
            var userPassword = new UserPassword
            {
                Password = await Task.Run(() => HashPassword(item.Password)),
                UserId = userId
            };
            await _userPasswordRepository.Create(userPassword);
            return userId;
        }

        public async Task Delete(Guid id)
        {
            await _userRepository.Delete(id);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
                throw new ErrorModel(400, "User not found");
            return user;
        }
        public async Task<User> GetByPhone(string phone)
        {
            return await _userRepository.GetByPhone(phone);
        }

        public async Task<User> Update(Guid id, User item)
        {
            return await _userRepository.Update(id, item);
        }

        public async Task<bool> Verification(string phoneNumber, string password)
        {
            var user = await _userRepository.GetByPhone(phoneNumber);
            if(user == null)
                return false;
            var userPassword = await _userPasswordRepository.GetByUserId(user.UserId);
            if(userPassword == null)
                return false;
            return VerifyHashedPassword(userPassword.Password, password);
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ErrorModel(406, "Argument Null");
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
                throw new ErrorModel(406, "Argument Null");
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
