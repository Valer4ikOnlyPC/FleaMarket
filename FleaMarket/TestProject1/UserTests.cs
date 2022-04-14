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
        private UserService _userService;

        public UserTests()
        {
            var mockUser = new Mock<IUserRepository>();
            var mockUserPassword = new Mock<IUserPasswordRepository>();
            _userService = new UserService(mockUser.Object, mockUserPassword.Object);
        }
        [Fact]
        public async Task HashPasswordTest()
        {
            //Arrange
            var password1 = _userService.HashPassword("bS39kr-1yd");
            //Act
            var password2 = _userService.HashPassword("bS39kr-1yd");
            //Assert
            Assert.NotEqual(password1, password2);
        }
        [Fact]
        public async Task VerifyHashedPasswordTest1()
        {
            //Arrange
            var password = "nD2_hY8`ta";
            var hashedPassword = _userService.HashPassword(password);
            //Act
            var result = _userService.VerifyHashedPassword(hashedPassword, password);
            //Assert
            Assert.True(result);
        }
        [Fact]
        public async Task VerifyHashedPasswordTest2()
        {
            //Arrange
            var password = "htd(b@mk%";
            var hashedPassword = _userService.HashPassword(password.ToUpper());
            //Act
            var result = _userService.VerifyHashedPassword(hashedPassword, password);
            //Assert
            Assert.False(result);
        }
        [Fact]
        public async Task VerifyHashedPasswordTest3()
        {
            //Arrange
            var password = _userService.HashPassword("up^N3$lEn");
            var hashedPassword = _userService.HashPassword(password);
            //Act
            var result = _userService.VerifyHashedPassword(hashedPassword, password);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task VerificationTest()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var mockUser = new Mock<IUserRepository>();
            mockUser.Setup(m => m.GetByPhone("88005553535")).ReturnsAsync(new User()
                { Name = "Name", Surname = "Surname", CityId = 1, IsDelete = false, PhoneNumber = "88005553535", Rating = 0, UserId = userId, VkAddress = null });
            var mockUserPassword = new Mock<IUserPasswordRepository>();
            mockUserPassword.Setup(m => m.GetByUserId(userId)).ReturnsAsync(new UserPassword()
                { Password = _userService.HashPassword("ftN%o<^>X"), UserId = userId, UserPasswordId = Guid.NewGuid() });
            var _userServiceTest = new UserService(mockUser.Object, mockUserPassword.Object);
            //Act
            var result = await _userServiceTest.Verification("88005553535", "ftN%o<^>X");
            //Assert
            Assert.True(result);
        }
    }
}
