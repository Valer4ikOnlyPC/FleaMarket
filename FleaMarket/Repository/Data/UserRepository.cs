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

namespace Repository.Data
{
    public class UserRepository: IUserRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        private IUserPasswordRepository _userPassword;
        public UserRepository(string conn, IUserPasswordRepository userPasswordRepository)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
            _userPassword = userPasswordRepository;
        }
        public IEnumerable<User> GetAll()
        {
            return db.Query<User>(
                "SELECT * " +
                "FROM Users").ToArray();
        }
        public User GetById(Guid id)
        {
            return db.Query<User>(
                "SELECT * " +
                "FROM Users " +
                "WHERE UserId = @id", new { id }).FirstOrDefault();
        }
        public Guid Create(User item, string password)
        {
            item.PasswordId = _userPassword.Create(password);

            return db.Query<Guid>(
                "INSERT INTO Users (Surname, Name1, VkAddress, Rating, CityId, IsDeleted, PasswordId) " +
                "VALUES(@Surname, @Name, @VkAddress, @Rating, @CityId, @IsDelete, @PasswordId) " +
                "RETURNING UserId;", new { item.Surname, item.Name, item.VkAddress, item.Rating, item.CityId, item.IsDelete, item.PasswordId }).FirstOrDefault();
        }
        public string Verification(string phoneNumber)
        {
            Guid passwordId = db.Query<Guid>(
                "SELECT PasswordId " +
                "FROM Users " +
                "WHERE PhoneNumber = @phoneNumber", new { phoneNumber }).FirstOrDefault();
            if (passwordId == new Guid())
                return "-1";
            return _userPassword.GetById(passwordId);
        }
        public User Update(Guid id, User item)
        {
            item.UserId = id;
            var sqlQuery =
                "UPDATE Users " +
                "SET Surname = @Surname, Name = @Name, VkAddress = @VkAddress, " +
                    "Rating = @Rating, CityId = @CityId, IsDelete = @IsDelete " +
                "WHERE UserId = @UserId";
            db.Execute(sqlQuery, item);
            return GetById(item.UserId);
        }
        public void Delete(Guid id)
        {
            var sqlQuery =
                "UPDATE Users " +
                "SET IsDelete = True " +
                "WHERE UserId = @id";
            db.Execute(sqlQuery, id);
        }
    }
}
