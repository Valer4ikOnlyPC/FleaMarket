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
        public Guid Create(string password)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

            return db.Query<Guid>(
                "INSERT INTO UserPasswords (\"Password\") " +
                "VALUES(@password) " +
                "RETURNING UserPasswordId;", new { password }).FirstOrDefault();
        }
        public string GetById(Guid passwordID)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));

            return db.Query<string>(
                "SELECT \"Password\" " +
                "FROM UserPasswords " +
                "WHERE UserPasswordId = @passwordID;", new { passwordID }).FirstOrDefault();
        }
    }
}
