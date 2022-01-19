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
    public class ProductPhotoRepository: IProductPhotoRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public ProductPhotoRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public IEnumerable<ProductPhoto> GetByProduct(Product product)
        {
            return db.Query<ProductPhoto>(
                "SELECT * " +
                "FROM ProductPhotos " +
                "WHERE ProductId = @ProductId", new { product.ProductId}).ToArray();
        }
        public ProductPhoto GetById(Guid id)
        {
            return db.Query<ProductPhoto>(
                "SELECT * " +
                "FROM ProductPhotos " +
                "WHERE PhotoId = @id", new { id }).FirstOrDefault();
        }
        public Guid Create(ProductPhoto item)
        {
            return db.Query<Guid>(
                "INSERT INTO ProductPhotos (Link, ProductId) " +
                "VALUES(@Link, @ProductId) " +
                "RETURNING ProductId;", new { item.Link, item.ProductId }).FirstOrDefault();
        }
        public ProductPhoto Update(Guid id, ProductPhoto item)
        {
            item.PhotoId = id;
            var sqlQuery =
                "UPDATE ProductPhotos " +
                "SET Link = @Link, ProductId = @ProductId " +
                "WHERE PhotoId = @PhotoId";
            db.Execute(sqlQuery, item);
            return GetById(item.PhotoId);
        }
        public void Delete(Guid id)
        {
            var sqlQuery =
                "DELETE FROM ProductPhotos " +
                "WHERE PhotoId = @id";
            db.Execute(sqlQuery, new { id });
        }
    }
}
