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
    public class UserRepository: BaseRepository, IUserRepository
    {
        private readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            var db = base.DbOpen(_configuration);
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM \"Users\"");
            base.DbClose(db);
            return users;
        }
        public async Task<User> GetById(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM \"Users\" " +
                "WHERE \"UserId\" = @id", new { id });
            base.DbClose(db);
            return users.FirstOrDefault();
        }
        public async Task<User> GetByPhone(string phone)
        {
            var db = base.DbOpen(_configuration);
            var users = await db.QueryAsync<User>(
                "SELECT * " +
                "FROM \"Users\" " +
                "WHERE \"PhoneNumber\" = @phone", new { phone });
            base.DbClose(db);
            return users.FirstOrDefault();
        }
        public async Task<Guid> Create(User item)
        {
            var db = base.DbOpen(_configuration);
            item.UserId = Guid.NewGuid();
            var result = await db.QueryAsync<Guid>(
                "INSERT INTO \"Users\" (\"UserId\", \"Surname\", \"Name\", \"PhoneNumber\", \"VkAddress\", \"Rating\", \"CityId\", \"IsDeleted\") " +
                "VALUES(@userId, @Surname, @Name, @PhoneNumber, @VkAddress, @Rating, @CityId, @IsDelete) " +
                "RETURNING \"UserId\";", new { item.UserId, item.Surname, item.Name, item.PhoneNumber, item.VkAddress, item.Rating, item.CityId, item.IsDelete });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task<User> Update(Guid id, User item)
        {
            var db = base.DbOpen(_configuration);
            item.UserId = id;
            var sqlQuery =
                "UPDATE \"Users\" " +
                "SET \"Surname\" = @Surname, \"Name\" = @Name, \"VkAddress\" = @VkAddress, " +
                    "\"Rating\" = @Rating, \"CityId\" = @CityId, \"IsDeleted\" = @IsDelete " +
                "WHERE \"UserId\" = @UserId";
            await db.ExecuteAsync(sqlQuery, new { item.UserId, item.Surname, item.Name, item.VkAddress, item.Rating, item.CityId, item.IsDelete });
            base.DbClose(db);
            return await GetById(item.UserId);
        }
        public async Task Delete(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Users\" " +
                "SET \"IsDeleted\" = true " +
                "WHERE \"UserId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }
    }
}
