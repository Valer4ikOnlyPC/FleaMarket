using Dapper;
using Domain.Core;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Controllers;
using Microsoft.Extensions.Configuration;
using Moq;
using Npgsql;
using Repository.Data;
using Services.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TestProject1
{
    public class UserTests
    {
        private string path;
        private Guid userId;
        private UserRepository _userRepository;
        private UserPasswordRepository _userPasswordRepository;
        private UserService _userService;

        public UserTests()
        {
            path = Directory.GetCurrentDirectory().Split("TestProject1").FirstOrDefault();
            var builder = new ConfigurationBuilder()
                .AddJsonFile(String.Concat(path, "/FleaMarket/appsettings.json"));
            var conf = builder.Build();
            _userRepository = new UserRepository(conf);
            _userPasswordRepository = new UserPasswordRepository(conf);
            _userService = new UserService(_userRepository, _userPasswordRepository);
            userId = Guid.Parse("e4993600-0b60-4111-b7cd-85b7097ced9c");
        }
        [Fact]
        public async Task TestCreate()
        {
            // Arrange
            var user = new UserDTO
            {
                Surname = "Пользователь",
                Name = "Тестовый",
                PhoneNumber = "+7 (922) 999 0001",
                CityId = 1,
                VkAddress = null,
                Password = "12345678",
            };
            // Act
            var userId = await _userService.Create(user);
            // Assert
            Assert.NotEqual(Guid.Empty, userId);
        }
        [Fact]
        public async Task TestGetById()
        {
            // Arrange
            var users = (await _userService.GetAll()).FirstOrDefault(u => u.UserId == userId);
            // Act
            var user = await _userService.GetById(userId);
            // Assert
            Assert.Equal(user.PhoneNumber, users.PhoneNumber);
        }
        [Fact]
        public async Task TestGetByPhone()
        {
            // Arrange
            var user1 = await _userService.GetById(userId);
            // Act
            var user2 = await _userService.GetByPhone(user1.PhoneNumber);
            // Assert
            Assert.Equal(user1.Name, user2.Name);
        }
        [Fact]
        public async Task TestUpdate()
        {
            // Arrange
            var user1 = await _userService.GetById(userId);
            user1.Name = "Обновление";
            // Act
            var user2 = await _userService.Update(userId, user1);
            // Assert
            Assert.Equal(user1.Name, user2.Name);
        }
        [Fact]
        public async Task TestVerification()
        {
            // Act
            var result = await _userService.Verification("+7 (910) 922 8989", "12345678");
            // Assert
            Assert.True(result);
        }
    }
}
