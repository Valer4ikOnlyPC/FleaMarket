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
    public class ProductPhotoRepository: IProductPhotoRepository
    {
        private readonly IConfiguration _configuration;
        public ProductPhotoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<ProductPhoto>> GetByProduct(Product product)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<ProductPhoto>(
                "SELECT * " +
                "FROM ProductPhotos " +
                "WHERE ProductId = @ProductId ", new { product.ProductId});
        }
        public async Task<ProductPhoto> GetById(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<ProductPhoto>(
                "SELECT * " +
                "FROM ProductPhotos " +
                "WHERE PhotoId = @id", new { id });
            return result.FirstOrDefault();
        }
        public async Task<Guid> Create(ProductPhoto item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Guid>(
                "INSERT INTO ProductPhotos (PhotoId, \"Link\", ProductId) " +
                "VALUES(@photoId, @Link, @ProductId) " +
                "RETURNING ProductId;", new { item.PhotoId, item.Link, item.ProductId });
            return result.FirstOrDefault();
        }
        public async Task<ProductPhoto> Update(Guid id, ProductPhoto item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            item.PhotoId = id;
            var sqlQuery =
                "UPDATE ProductPhotos " +
                "SET \"Link\" = @Link, ProductId = @ProductId " +
                "WHERE PhotoId = @PhotoId";
            await db.ExecuteAsync(sqlQuery, item);
            return await GetById(item.PhotoId);
        }
        public async void Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM ProductPhotos " +
                "WHERE PhotoId = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
        }
    }
}
