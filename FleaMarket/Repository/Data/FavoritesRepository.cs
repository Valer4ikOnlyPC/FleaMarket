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
    public class FavoritesRepository: IFavoritesRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public FavoritesRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public IEnumerable<Favorite> GetAll()
        {
            return db.Query<Favorite>(
                "SELECT * " +
                "FROM Favorites").ToArray();
        }
        public IEnumerable<Favorite> GetByUser(User user)
        {
            return db.Query<Favorite>(
                "SELECT * " +
                "FROM Favorites " +
                "WHERE UserId = @UserId", new { user.UserId  }).ToArray();
        }
        public Favorite GetById(Guid id)
        {
            return db.Query<Favorite>(
                "SELECT * " +
                "FROM Favorites " +
                "WHERE FavoriteId = @id", new { id }).FirstOrDefault();
        }
        public Guid Create(Favorite item)
        {
            return db.Query<Guid>(
                "INSERT INTO Favorites (ProductId, UserId) " +
                "VALUES(@ProductId, @UserId) " +
                "RETURNING FavoriteId;", new { item.ProductId, item.UserId }).FirstOrDefault();
        }
        public void Delete(Guid id)
        {
            var sqlQuery =
                "DELETE FROM Favorites " +
                "WHERE FavoriteId = @id";
            db.Execute(sqlQuery, new { id });
        }
    }
}
