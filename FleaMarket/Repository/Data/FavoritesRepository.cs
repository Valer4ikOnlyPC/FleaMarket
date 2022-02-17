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
    public class FavoritesRepository: IFavoritesRepository
    {
        private readonly IConfiguration _configuration;
        public FavoritesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Favorite>> GetAll()
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Favorite>(
                "SELECT * " +
                "FROM \"Favorites\"");
        }
        public async Task<IEnumerable<Favorite>> GetByUser(User user)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Favorite>(
                "SELECT * " +
                "FROM \"Favorites\" " +
                "WHERE \"UserId\" = @UserId", new { user.UserId  });
        }
        public async Task<Favorite> GetById(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var favorite = await db.QueryAsync<Favorite>(
                "SELECT * " +
                "FROM \"Favorites\" " +
                "WHERE \"FavoriteId\" = @id", new { id });
            return favorite.FirstOrDefault();
        }
        public async Task<Guid> Create(Favorite item)
        {
            item.FavoriteId = Guid.NewGuid();
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var favorite = await db.QueryAsync<Guid>(
                "INSERT INTO \"Favorites\" (\"FavoriteId\", \"ProductId\", \"UserId\") " +
                "VALUES(@FavoriteId, @ProductId, @UserId) " +
                "RETURNING \"FavoriteId\";", new { item.FavoriteId, item.ProductId, item.UserId });
            return favorite.FirstOrDefault();
        }
        public async void Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM \"Favorites\" " +
                "WHERE \"FavoriteId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
        }
    }
}
