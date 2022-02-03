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
    public class DealRepository: IDealRepository
    {
        private readonly IConfiguration _configuration;
        public DealRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Deal>> GetAll()
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM Deals");
        }
        public async Task<IEnumerable<Deal>> GetByMaster(User userMaster)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM Deals " +
                "WHERE UserMaster = @UserId", new { userMaster.UserId });
        }
        public async Task<IEnumerable<Deal>> GetByRecipient(User userRecipient)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM Deals " +
                "WHERE UserRecipient = @UserId", new { userRecipient.UserId });
        }
        public async Task<Deal> GetById(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var deal = await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM Deals " +
                "WHERE DealId = @id", new { id });
            return deal.FirstOrDefault();
        }
        // продукт isactive = InDeal
        public async Task<Guid> Create(Deal item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var deal = await db.QueryAsync<Guid>(
               "INSERT INTO Deals (UserMaster, ProductMaster, UserRecipient, ProductRecipient, IsActive) " +
               "VALUES(@UserMaster, @ProductMaster, @UserRecipient, @ProductRecipient, @IsActive) " +
               "RETURNING DealId;", new { item.UserMaster, item.ProductMaster, item.UserRecipient, item.ProductRecipient, item.IsActive });
            return deal.FirstOrDefault();
        }
        //добавлять в историю + закрывать продукт 
        public async void Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM Deals " +
                "WHERE DealId = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            // +добавлять в историю
        }
    }
}
