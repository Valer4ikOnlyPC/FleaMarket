using Dapper;
using Domain.Models;
using Npgsql;
using Domain.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Repository.Data
{
    public class UserRepository: IUserRepository
    {
        private IUserPasswordRepository _userPassword;

        private readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration, IUserPasswordRepository userPasswordRepository)
        {
            _configuration = configuration;
            _userPassword = userPasswordRepository;
        }
        public IEnumerable<User> GetAll()
        {
            string constr = _configuration.GetConnectionString("myconn");
            IDbConnection db = new NpgsqlConnection(constr);
            return db.Query<User>(
                "SELECT * " +
                "FROM Users").ToArray();
        }
        public async Task<User> GetById(Guid id)
        {
            string constr = _configuration.GetConnectionString("myconn");
            IDbConnection db = new NpgsqlConnection(constr);
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM Users " +
                "WHERE UserId = @id", new { id });
            return users.FirstOrDefault();
        }
        public async Task<User> GetByPhone(string phone)
        {
            string constr = _configuration.GetConnectionString("myconn");
            IDbConnection db = new NpgsqlConnection(constr);
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM Users " +
                "WHERE PhoneNumber = @phone", new { phone });
            return users.FirstOrDefault();
        }
        public async Task<Guid> Create(User item, string password)
        {
            string constr = _configuration.GetConnectionString("myconn");
            IDbConnection db = new NpgsqlConnection(constr);
            item.PasswordId = _userPassword.Create(password);

            var result = await db.QueryAsync<Guid>(
                "INSERT INTO Users (Surname, \"Name\", PhoneNumber, VkAddress, Rating, CityId, IsDeleted, PasswordId) " +
                "VALUES(@Surname, @Name, @PhoneNumber, @VkAddress, @Rating, @CityId, @IsDelete, @PasswordId) " +
                "RETURNING UserId;", new { item.Surname, item.Name, item.PhoneNumber, item.VkAddress, item.Rating, item.CityId, item.IsDelete, item.PasswordId });
            return result.FirstOrDefault();
        }
        public async Task<string> Verification(string phoneNumber)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var passwordId = await db.QueryAsync<Guid>(
                "SELECT PasswordId " +
                "FROM Users " +
                "WHERE PhoneNumber = @phoneNumber", new { phoneNumber });
            if (passwordId.FirstOrDefault() == new Guid())
                return "-1";
            return _userPassword.GetById(passwordId.FirstOrDefault());
        }
        public async Task<User> Update(Guid id, User item)
        {
            string constr = _configuration.GetConnectionString("myconn");
            IDbConnection db = new NpgsqlConnection(constr);

            item.UserId = id;
            var sqlQuery =
                "UPDATE Users " +
                "SET Surname = @Surname, Name = @Name, VkAddress = @VkAddress, " +
                    "Rating = @Rating, CityId = @CityId, IsDelete = @IsDelete " +
                "WHERE UserId = @UserId";
            await db.ExecuteAsync(sqlQuery, item);
            return await GetById(item.UserId);
        }
        public async void Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

            var sqlQuery =
                "UPDATE Users " +
                "SET IsDelete = True " +
                "WHERE UserId = @id";
            await db.ExecuteAsync(sqlQuery, id);
        }
    }
}
