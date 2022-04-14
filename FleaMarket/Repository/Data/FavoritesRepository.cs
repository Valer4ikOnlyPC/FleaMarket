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
    public class FavoritesRepository: BaseRepository, IFavoritesRepository
    {
        private readonly IConfiguration _configuration;
        public FavoritesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Favorite>> GetAll()
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Favorite>(
                "SELECT * " +
                "FROM \"Favorites\"");
            base.DbClose(db);
            return result;
        }
        public async Task<IEnumerable<Favorite>> GetByUser(User user)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Favorite>(
                "SELECT * " +
                "FROM \"Favorites\" " +
                "WHERE \"UserId\" = @UserId", new { user.UserId  });
            base.DbClose(db);
            return result;
        }
        public async Task<Favorite> GetById(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var favorite = await db.QueryAsync<Favorite>(
                "SELECT * " +
                "FROM \"Favorites\" " +
                "WHERE \"FavoriteId\" = @id", new { id });
            base.DbClose(db);
            return favorite.FirstOrDefault();
        }
        public async Task<Guid> Create(Favorite item)
        {
            item.FavoriteId = Guid.NewGuid();
            var db = base.DbOpen(_configuration);
            var favorite = await db.QueryAsync<Guid>(
                "INSERT INTO \"Favorites\" (\"FavoriteId\", \"ProductId\", \"UserId\") " +
                "VALUES(@FavoriteId, @ProductId, @UserId) " +
                "RETURNING \"FavoriteId\";", new { item.FavoriteId, item.ProductId, item.UserId });
            base.DbClose(db);
            return favorite.FirstOrDefault();
        }
        public async Task Delete(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "DELETE FROM \"Favorites\" " +
                "WHERE \"FavoriteId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }
    }
}
