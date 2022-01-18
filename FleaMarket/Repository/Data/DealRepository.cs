using Dapper;
using Domain.Models;
using Npgsql;
using Repository.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data
{
    public class DealRepository: IDealRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public DealRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public IEnumerable<Deal> GetAll()
        {
            return db.Query<Deal>(
                "SELECT * " +
                "FROM Deals").ToArray();
        }
        public IEnumerable<Deal> GetByMaster(User userMaster)
        {
            return db.Query<Deal>(
                "SELECT * " +
                "FROM Deals " +
                "WHERE UserMaster = @UserId", new { userMaster.UserId }).ToArray();
        }
        public IEnumerable<Deal> GetByRecipient(User userRecipient)
        {
            return db.Query<Deal>(
                "SELECT * " +
                "FROM Deals " +
                "WHERE UserRecipient = @UserId", new { userRecipient.UserId }).ToArray();
        }
        public Deal GetById(Guid id)
        {
            return db.Query<Deal>(
                "SELECT * " +
                "FROM Deals " +
                "WHERE DealId = @id", new { id }).FirstOrDefault();
        }
        // продукт isactive = InDeal
        public Guid Create(Deal item)
        {
            return db.Query<Guid>(
               "INSERT INTO Deals (UserMaster, ProductMaster, UserRecipient, ProductRecipient, IsActive) " +
               "VALUES(@UserMaster, @ProductMaster, @UserRecipient, @ProductRecipient, @IsActive) " +
               "RETURNING DealId;", new { item.UserMaster, item.ProductMaster, item.UserRecipient, item.ProductRecipient, item.IsActive }).FirstOrDefault();
        }
        //добавлять в историю + закрывать продукт 
        public void Delete(Guid id)
        {
            var sqlQuery =
                "DELETE FROM Deals " +
                "WHERE DealId = @id";
            db.Execute(sqlQuery, new { id });
            // +добавлять в историю
        }
    }
}
