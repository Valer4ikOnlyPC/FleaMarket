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
                "FROM Ratings " +
                "WHERE RatingId = @UserId", new { user.UserId });
        }
        //менять оценку в пользователе
        public async Task<Guid> Create(Rating item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var ratingId = await db.QueryAsync<Guid>(
                "INSERT INTO Ratings (Grade, UserId) " +
                "VALUES(@Grade, @UserId) " +
                "RETURNING RatingId;", new { item.Grade, item.UserId });

            var sqlQuery =
                "UPDATE Users " +
                "SET Rating = (SELECT AVG(Grade) FROM Ratings WHERE UserId = @UserId) " +
                "WHERE UserId = @UserId";
            db.ExecuteAsync(sqlQuery, item.UserId);

            return ratingId.FirstOrDefault();
        }
        public async void Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM Ratings " +
                "WHERE RatingId = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
        }
    }
}
