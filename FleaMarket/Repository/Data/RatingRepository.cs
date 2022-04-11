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
    public class RatingRepository: BaseRepository, IRatingRepository
    {
        private readonly IConfiguration _configuration;
        public RatingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Rating>> GetByUser(User user)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Rating>(
                "SELECT * " +
                "FROM \"Ratings\" " +
                "WHERE \"UserRecipientId\" = @UserId", new { user.UserId });
            base.DbClose(db);
            return result;
        }
        public async Task<IEnumerable<Rating>> GetByDeal(Deal deal)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Rating>(
                "SELECT * " +
                "FROM \"Ratings\" " +
                "WHERE (\"UserMasterId\" = @UserMaster or \"UserMasterId\" = @UserRecipient) and \"DealId\" = @DealId", new { deal.UserMaster, deal.UserRecipient, deal.DealId });
            base.DbClose(db);
            return result;
        }
        public async Task<Guid> Create(Rating item)
        {
            item.RatingId = Guid.NewGuid();
            var db = base.DbOpen(_configuration);
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
            base.DbClose(db);
            return ratingId.FirstOrDefault();
        }
        public async Task Delete(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "DELETE FROM \"Ratings\" " +
                "WHERE \"RatingId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }
    }
}
