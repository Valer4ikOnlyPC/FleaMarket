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
        private readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM Users");
            return users;
        }
        public async Task<User> GetById(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM Users " +
                "WHERE UserId = @id", new { id });
            return users.FirstOrDefault();
        }
        public async Task<User> GetByPhone(string phone)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM Users " +
                "WHERE PhoneNumber = @phone", new { phone });
            return users.FirstOrDefault();
        }
        public async Task<Guid> Create(User item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

            item.UserId = Guid.NewGuid();
            var result = await db.QueryAsync<Guid>(
                "INSERT INTO Users (UserId, Surname, \"Name\", PhoneNumber, VkAddress, Rating, CityId, IsDeleted) " +
                "VALUES(@userId, @Surname, @Name, @PhoneNumber, @VkAddress, @Rating, @CityId, @IsDelete) " +
                "RETURNING UserId;", new { item.UserId, item.Surname, item.Name, item.PhoneNumber, item.VkAddress, item.Rating, item.CityId, item.IsDelete });
            return result.FirstOrDefault();
        }
        public async Task<User> Update(Guid id, User item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

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
