using Domain.Core;
using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using System.Security.Cryptography;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Repository.Data
{
    public class UserPasswordRepository: IUserPasswordRepository
    {
        private readonly IConfiguration _configuration;
        public UserPasswordRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Guid> Create(UserPassword userPassword)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

            userPassword.UserPasswordId = Guid.NewGuid();
            var passwordId = await db.QueryAsync<Guid>(
                "INSERT INTO UserPasswords (UserPasswordId, \"Password\", UserId) " +
                "VALUES(@userPasswordId, @password, @userId) " +
                "RETURNING UserPasswordId;", new { userPassword.UserPasswordId, userPassword.Password, userPassword.UserId });
            return passwordId.FirstOrDefault();
        }
        public async Task<UserPassword> GetById(Guid passwordID)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

            var userPassword = await db.QueryAsync<UserPassword>(
                "SELECT * " +
                "FROM UserPasswords " +
                "WHERE UserPasswordId = @passwordID;", new { passwordID });
            return userPassword.FirstOrDefault();
        }
        public async Task<UserPassword> GetByUserId(Guid userID)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

            var userPassword = await db.QueryAsync<UserPassword>(
                "SELECT * " +
                "FROM UserPasswords " +
                "WHERE UserId = @userID;", new { userID });
            return userPassword.FirstOrDefault();
        }
    }
}
