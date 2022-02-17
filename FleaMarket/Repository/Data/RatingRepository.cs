using Domain.Models;
using Domain.Core;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Repository.Data
{
    public class RatingRepository: IRatingRepository
    {
        private readonly IConfiguration _configuration;
        public RatingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Rating>> GetByUser(User user)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Rating>(
                "SELECT * " +
                "FROM \"Ratings\" " +
                "WHERE \"UserRecipientId\" = @UserId", new { user.UserId });
        }
        public async Task<IEnumerable<Rating>> GetByDeal(Deal deal)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Rating>(
                "SELECT * " +
                "FROM \"Ratings\" " +
                "WHERE (\"UserMasterId\" = @UserMaster or \"UserMasterId\" = @UserRecipient) and \"DealId\" = @DealId", new { deal.UserMaster, deal.UserRecipient, deal.DealId });
        }
        public async Task<Guid> Create(Rating item)
        {
            item.RatingId = Guid.NewGuid();
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var ratingId = await db.QueryAsync<Guid>(
                "INSERT INTO \"Ratings\" (\"RatingId\", \"Grade\", \"UserMasterId\", \"UserRecipientId\", \"DealId\") " +
                "VALUES(@RatingId, @Grade, @UserMasterId, @UserRecipientId, @DealId) " +
                "RETURNING \"RatingId\";", new { item.RatingId, item.Grade, item.UserMasterId, item.UserRecipientId, item.DealId });

            var rating = await db.QueryAsync<float>(
                "SELECT AVG(\"Grade\") " +
                "FROM \"Ratings\" " +
                "WHERE \"UserRecipientId\" = @UserRecipientId; ", new { item.UserRecipientId });

            var sqlQuery =
                "UPDATE \"Users\" " +
                "SET \"Rating\" = @ratings " +
                "WHERE \"UserId\" = @UserRecipientId";
            await db.ExecuteAsync(sqlQuery,new { ratings = rating.FirstOrDefault(), item.UserRecipientId });

            return ratingId.FirstOrDefault();
        }
        public async void Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM \"Ratings\" " +
                "WHERE \"RatingId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
        }
    }
}
