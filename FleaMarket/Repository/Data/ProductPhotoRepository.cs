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
    public class ProductPhotoRepository: BaseRepository, IProductPhotoRepository
    {
        private readonly IConfiguration _configuration;
        public ProductPhotoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<ProductPhoto>> GetByProduct(Guid productId)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<ProductPhoto>(
                "SELECT * " +
                "FROM \"ProductPhotos\" " +
                "WHERE \"ProductId\" = @ProductId ", new { productId });
            base.DbClose(db);
            return result;
        }
        public async Task<ProductPhoto> GetById(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<ProductPhoto>(
                "SELECT * " +
                "FROM \"ProductPhotos\" " +
                "WHERE \"PhotoId\" = @id", new { id });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task<Guid> Create(ProductPhoto item)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Guid>(
                "INSERT INTO \"ProductPhotos\" (\"PhotoId\", \"Link\", \"ProductId\") " +
                "VALUES(@photoId, @Link, @ProductId) " +
                "RETURNING \"ProductId\";", new { item.PhotoId, item.Link, item.ProductId });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task<ProductPhoto> Update(Guid id, ProductPhoto item)
        {
            var db = base.DbOpen(_configuration);
            item.PhotoId = id;
            var sqlQuery =
                "UPDATE \"ProductPhotos\" " +
                "SET \"Link\" = @Link, \"ProductId\" = @ProductId " +
                "WHERE \"PhotoId\" = @PhotoId";
            await db.ExecuteAsync(sqlQuery, item);
            base.DbClose(db);
            return await GetById(item.PhotoId);
        }
        public async Task Delete(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "DELETE FROM \"ProductPhotos\" " +
                "WHERE \"PhotoId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }
    }
}
