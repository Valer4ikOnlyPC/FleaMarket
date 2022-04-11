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
    public class UserPasswordRepository: BaseRepository, IUserPasswordRepository
    {
        private readonly IConfiguration _configuration;
        public UserPasswordRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Guid> Create(UserPassword userPassword)
        {
            var db = base.DbOpen(_configuration);
            userPassword.UserPasswordId = Guid.NewGuid();
            var passwordId = await db.QueryAsync<Guid>(
                "INSERT INTO \"UserPasswords\" (\"UserPasswordId\", \"Password\", \"UserId\") " +
                "VALUES(@userPasswordId, @password, @userId) " +
                "RETURNING \"UserPasswordId\";", new { userPassword.UserPasswordId, userPassword.Password, userPassword.UserId });
            base.DbClose(db);
            return passwordId.FirstOrDefault();
        }
        public async Task<UserPassword> GetById(Guid passwordID)
        {
            var db = base.DbOpen(_configuration);
            var userPassword = await db.QueryAsync<UserPassword>(
                "SELECT * " +
                "FROM \"UserPasswords\" " +
                "WHERE \"UserPasswordId\" = @passwordID;", new { passwordID });
            base.DbClose(db);
            return userPassword.FirstOrDefault();
        }
        public async Task<UserPassword> GetByUserId(Guid userID)
        {
            var db = base.DbOpen(_configuration);
            var userPassword = await db.QueryAsync<UserPassword>(
                "SELECT * " +
                "FROM \"UserPasswords\" " +
                "WHERE \"UserId\" = @userID;", new { userID });
            base.DbClose(db);
            return userPassword.FirstOrDefault();
        }
    }
}
