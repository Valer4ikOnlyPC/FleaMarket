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
using Domain.Dto;

namespace Repository.Data
{
    public class ProductRepository : BaseRepository, IProductRepository 
    {
        private readonly IConfiguration _configuration;
        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Product>> GetAll(string categories = null, int page = 0, int cityId = -1)
        {
            var db = base.DbOpen(_configuration);//new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var query = "SELECT * " +
                "FROM \"Products\" " +
                "WHERE \"IsActive\" = 2 ";
            if (categories != null) query = string.Concat(query, $"AND \"CategoryId\" IN {categories} ");
            if (cityId != -1) query = string.Concat(query, $"AND \"CityId\" = {cityId} ");
            query = string.Concat(query, $"OFFSET {30 * page} LIMIT 30");
            var product = await db.QueryAsync<Product>(query);
            base.DbClose(db);
            return product;
        }
        public async Task<int> GetCountAll(string categories = null, int cityId = -1)
        {
            var db = base.DbOpen(_configuration);
            var query = "SELECT COUNT(*) " +
                "FROM \"Products\" " +
                $"WHERE \"IsActive\" = 2 ";
            if (categories != null) query = string.Concat(query, $"AND \"CategoryId\" IN {categories}");
            if (cityId != -1) query = string.Concat(query, $"AND \"CityId\" = {cityId}");
            var productCount = await db.QueryAsync<int>(query);
            base.DbClose(db);
            return productCount.FirstOrDefault();
        }
        public async Task<IEnumerable<Product>> GetBySearch(string search, string categories = null, int page = 0, int cityId = -1)
        {
            var db = base.DbOpen(_configuration);
            var query = "SELECT *, ts_rank_cd(vector, query) AS rank " +
                $"FROM \"Products\", to_tsquery('russian', '{search}') query, to_tsvector('russian', \"Name\" ||' '|| \"Description\") vector " +
                $"WHERE  query @@ vector AND \"IsActive\" = 2 ";
            if (categories != null) query = string.Concat(query, $"AND \"CategoryId\" IN {categories} ");
            if (cityId != -1) query = string.Concat(query, $"AND \"CityId\" = {cityId} ");
            query = string.Concat(query, "ORDER BY rank DESC ");
            query = string.Concat(query, $"OFFSET {30 * page} LIMIT 30 ");
            var result = await db.QueryAsync<Product>(query);
            base.DbClose(db);
            return result;
        }
        public async Task<int> GetCountAllBySearch(string search, string categories = null, int cityId = -1)
        {
            var db = base.DbOpen(_configuration);
            var query = "SELECT COUNT(*) " +
                $"FROM \"Products\", to_tsquery('russian', '{search}') query, to_tsvector('russian', \"Name\" ||' '|| \"Description\") vector " +
                $"WHERE  query @@ vector AND \"IsActive\" = 2 ";
            if (categories != null) query = string.Concat(query, $"AND \"CategoryId\" IN {categories} ");
            if (cityId != -1) query = string.Concat(query, $"AND \"CityId\" = {cityId} ");
            var result = await db.QueryAsync<int>(query);
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task<IEnumerable<Product>> GetByUser(User user)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Product>(
                "SELECT * " +
                "FROM \"Products\" " +
                "WHERE \"UserId\" = @UserId", new { user.UserId });
            base.DbClose(db);
            return result;
        }
        public async Task<Product> GetById(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Product>(
                "SELECT * " +
                "FROM \"Products\" " +
                "WHERE \"ProductId\" = @id", new { id });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task<Guid> Create(Product item)
        {
            item.Date = DateTime.Now;
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Guid>(
                "INSERT INTO \"Products\" (\"ProductId\", \"Name\", \"FirstPhoto\", \"Description\", \"CityId\", \"IsActive\", \"CategoryId\", \"Date\", \"UserId\") " +
                "VALUES(@ProductId, @Name, @FirstPhoto, @Description, @CityId, 2, @CategoryId, @Date, @UserId) " +
                "RETURNING \"ProductId\";", new { item.ProductId, item.Name, item.FirstPhoto, item.Description, item.CityId, item.CategoryId, item.Date, item.UserId });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task UpdatePhoto(Guid id, string Photo)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"FirstPhoto\" = @Photo " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { Photo, id});
            base.DbClose(db);
        }
        public async Task<Product> Update(Guid id, Product item)
        {
            var db = base.DbOpen(_configuration);
            item.ProductId = id;
            var result = await db.QueryAsync<Guid>(
                "UPDATE \"Products\" " +
                "SET \"Name\" = @Name, \"Description\" = @Description, \"CityId\" = @CityId, \"CategoryId\" = @CategoryId " +
                "WHERE \"ProductId\" = @id " +
                "RETURNING \"ProductId\";", new { item.Name, item.Description, item.CityId, item.CategoryId, id });
            base.DbClose(db);
            return await GetById(result.FirstOrDefault());
        }
        public async Task UpdateState(Guid id, int number)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"IsActive\" = @number " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { number, id });
            base.DbClose(db);
        }
        public async Task DealCompleted(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"IsActive\" = 3 " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, id);
            base.DbClose(db);
        }
        public async Task Delete(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"IsActive\" = 0 " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }
        public async Task<IEnumerable<Product>> GetByCategory(int categoryId)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Product>(
                "SELECT * " +
                "FROM \"Products\" " +
                "WHERE \"CategoryId\" = @CategoryId", new { categoryId });
            base.DbClose(db);
            return result.Where(p => p.IsActive == ProductState.Active);
        }
    }
}
