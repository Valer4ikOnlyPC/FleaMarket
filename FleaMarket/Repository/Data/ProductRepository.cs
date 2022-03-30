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
    public class ProductRepository: IProductRepository
    {
        private readonly IConfiguration _configuration;
        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Product>> GetAll()
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var product = await db.QueryAsync<Product>(
                "SELECT * " +
                "FROM \"Products\" ");
            return product;
        }
        public async Task<IEnumerable<Product>> GetByUser(User user)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Product>(
                "SELECT * " +
                "FROM \"Products\" " +
                "WHERE \"UserId\" = @UserId", new { user.UserId });
            return result;
        }
        public async Task<Product> GetById(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Product>(
                "SELECT * " +
                "FROM \"Products\" " +
                "WHERE \"ProductId\" = @id", new { id });
            return result.FirstOrDefault();
        }
        public async Task<Guid> Create(Product item)
        {
            item.Date = DateTime.Now;
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Guid>(
                "INSERT INTO \"Products\" (\"ProductId\", \"Name\", \"FirstPhoto\", \"Description\", \"CityId\", \"IsActive\", \"CategoryId\", \"Date\", \"UserId\") " +
                "VALUES(@ProductId, @Name, @FirstPhoto, @Description, @CityId, 2, @CategoryId, @Date, @UserId) " +
                "RETURNING \"ProductId\";", new { item.ProductId, item.Name, item.FirstPhoto, item.Description, item.CityId, item.CategoryId, item.Date, item.UserId });
            return result.FirstOrDefault();
        }
        public async Task UpdatePhoto(Guid id, string Photo)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"FirstPhoto\" = @Photo " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { Photo, id});
        }
        public async Task<Product> Update(Guid id, Product item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            item.ProductId = id;
            var result = await db.QueryAsync<Guid>(
                "UPDATE \"Products\" " +
                "SET \"Name\" = @Name, \"Description\" = @Description, \"CityId\" = @CityId, \"CategoryId\" = @CategoryId " +
                "WHERE \"ProductId\" = @id " +
                "RETURNING \"ProductId\";", new { item.Name, item.Description, item.CityId, item.CategoryId, id });
            return await GetById(result.FirstOrDefault());
        }
        public async Task UpdateState(Guid id, int number)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"IsActive\" = @number " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { number, id });
        }
        public async Task DealCompleted(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"IsActive\" = 3 " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, id);
        }
        public async Task Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "UPDATE \"Products\" " +
                "SET \"IsActive\" = 0 " +
                "WHERE \"ProductId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
        }
        public async Task<IEnumerable<Product>> GetByCategory(int categoryId)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Product>(
                "SELECT * " +
                "FROM \"Products\" " +
                "WHERE \"CategoryId\" = @CategoryId", new { categoryId });
            return result.Where(p => p.IsActive == ProductState.Active);
        }
        public async Task<IEnumerable<Product>> GetBySearch(string search)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Product>(
                "SELECT *, ts_rank_cd(vector, query) AS rank " +
                "FROM \"Products\", to_tsquery('russian', @search) query, to_tsvector('russian', \"Name\" ||' '|| \"Description\") vector " +
                "WHERE query @@ vector " +
                "ORDER BY rank DESC", new { search });
            return result.Where(p => p.IsActive == ProductState.Active);
        }
    }
}
