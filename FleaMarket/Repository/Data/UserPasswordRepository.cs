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

namespace Repository.Data
{
    public class UserPasswordRepository: IUserPasswordRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public UserPasswordRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public Guid Create(string password)
        {
            return db.Query<Guid>(
                "INSERT INTO UserPasswords (Password1) " +
                "VALUES(@Password) " +
                "RETURNING UserPasswordId;", new { password }).FirstOrDefault();
        }
        public string GetById(Guid passwordID)
        {
            return db.Query<string>(
                "SELECT Password1 " +
                "FROM UserPasswords " +
                "WHERE UserPasswordId = @passwordID;", new { passwordID }).FirstOrDefault();
        }
    }
}
