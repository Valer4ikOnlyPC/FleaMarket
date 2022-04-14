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
    public class DealRepository: BaseRepository, IDealRepository
    {
        private readonly IConfiguration _configuration;
        public DealRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Deal>> GetAll()
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM \"Deals\"");
            base.DbClose(db);
            return result;
        }
        public async Task<IEnumerable<Deal>> GetByMaster(User userMaster)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM \"Deals\" " +
                "WHERE \"UserMaster\" = @UserId", new { userMaster.UserId });
            base.DbClose(db);
            return result;
        }
        public async Task<IEnumerable<Deal>> GetByRecipient(User userRecipient)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM \"Deals\" " +
                "WHERE \"UserRecipient\" = @UserId", new { userRecipient.UserId });
            base.DbClose(db);
            return result;
        }
        public async Task<IEnumerable<Deal>> GetByUser(User user)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM \"Deals\" " +
                "WHERE \"UserRecipient\" = @UserId or \"UserMaster\" = @UserId", new { user.UserId });
            base.DbClose(db);
            return result;
        }
        public async Task<int> GetByRecipientCount(User userRecipient)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<int>(
                "SELECT Count(*) " +
                "FROM \"Deals\" " +
                "WHERE \"UserRecipient\" = @UserId and \"IsActive\" = 0", new { userRecipient.UserId });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task<IEnumerable<Deal>> GetByProduct(Guid productId)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM \"Deals\" " +
                "WHERE \"ProductRecipient\" = @productId or \"ProductMaster\" = @productId", new { productId });
            base.DbClose(db);
            return result;
        }
        public async Task<Deal> GetById(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var deal = await db.QueryAsync<Deal>(
                "SELECT * " +
                "FROM \"Deals\" " +
                "WHERE \"DealId\" = @id", new { id });
            base.DbClose(db);
            return deal.FirstOrDefault();
        }
        public async Task<Guid> Create(Deal item)
        {
            item.DealId = Guid.NewGuid();
            item.Date = DateTime.Now;
            var db = base.DbOpen(_configuration);
            var deal = await db.QueryAsync<Guid>(
               "INSERT INTO \"Deals\" (\"DealId\", \"UserMaster\", \"ProductMaster\", \"UserRecipient\", \"ProductRecipient\", \"IsActive\", \"Date\") " +
               "VALUES(@DealId, @UserMaster, @ProductMaster, @UserRecipient, @ProductRecipient, 0, @Date) " +
               "RETURNING \"DealId\";", new { item.DealId, item.UserMaster, item.ProductMaster, item.UserRecipient, item.ProductRecipient, item.Date });
            base.DbClose(db);
            return deal.FirstOrDefault();
        }
        public async Task UpdateDate(Guid id)
        {
            var date = DateTime.Now;
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Deals\" " +
                "SET \"Date\" = @date " +
                "WHERE \"DealId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id, date });
            base.DbClose(db);
        }
        public async Task Update(Guid id, int number)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Deals\" " +
                "SET \"IsActive\" = @number " +
                "WHERE \"DealId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { number, id });
            base.DbClose(db);
        }
 
        public async Task Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM \"Deals\" " +
                "WHERE \"DealId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }
    }
}
