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

namespace Repository.Data
{
    public class RatingRepository: IRatingRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public RatingRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public IEnumerable<Rating> GetByUser(User user)
        {
            return db.Query<Rating>(
                "SELECT * " +
                "FROM Ratings " +
                "WHERE RatingId = @UserId", new { user.UserId }).ToArray();
        }
        //менять оценку в пользователе
        public Guid Create(Rating item)
        {
            Guid ratingId = db.Query<Guid>(
                "INSERT INTO Ratings (Grade, UserId) " +
                "VALUES(@Grade, @UserId) " +
                "RETURNING RatingId;", new { item.Grade, item.UserId }).FirstOrDefault();

            var sqlQuery =
                "UPDATE Users " +
                "SET Rating = (SELECT AVG(Grade) FROM Ratings WHERE UserId = @UserId) " +
                "WHERE UserId = @UserId";
            db.Execute(sqlQuery, item);

            return ratingId;
        }
        public void Delete(Guid id)
        {
            var sqlQuery =
                "DELETE FROM Ratings " +
                "WHERE RatingId = @id";
            db.Execute(sqlQuery, new { id });
        }
    }
}
