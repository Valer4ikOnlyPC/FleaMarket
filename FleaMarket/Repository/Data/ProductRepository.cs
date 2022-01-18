using Domain.Models;
using Repository.Core;
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
    public class ProductRepository: IProductRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public ProductRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public IEnumerable<Product> GetAll()
        {
            return db.Query<Product>(
                "SELECT * " +
                "FROM Products").Where(p => p.IsActive == Product.enumIsActive.Active).ToArray();
        }
        public IEnumerable<Product> GetByUser(User user)
        {
            return db.Query<Product>(
                "SELECT * " +
                "FROM Products " +
                "WHERE UserId = @UserId", new { user.UserId }).Where(p => p.IsActive == Product.enumIsActive.Active).ToArray();
        }
        public Product GetById(Guid id)
        {
            return db.Query<Product>(
                "SELECT * " +
                "FROM Products " +
                "WHERE ProductId = @id", new { id }).FirstOrDefault(p => p.IsActive == Product.enumIsActive.Active);
        }
        public Guid Create(Product item)
        {
            return db.Query<Guid>(
                "INSERT INTO Products (Name, FirstPhoto, Description, CityId, IsActive, CategoryId, UserId) " +
                "VALUES(@Name, @FirstPhoto, @Description, @CityId, @IsActive, @CategoryId, @UserId) " +
                "RETURNING ProductId;", new { item.Name, item.FirstPhoto, item.Description, item.CityId, Product.enumIsActive.Active, item.CategoryId, item.UserId }).FirstOrDefault();
        }
        public Product Update(Guid id, Product item)
        {
            item.ProductId = id;
            var sqlQuery =
                "UPDATE Products " +
                "SET Name = @Name, FirstPhoto = @FirstPhoto, Description = @Description, " +
                    "City = @City, CategoryId = @CategoryId, UserId = @UserId " +
                "WHERE ProductId = @ProductId";
            db.Execute(sqlQuery, item);
            return GetById(item.ProductId);
        }
        public void Delete(Guid id)
        {
            var sqlQuery =
                "UPDATE Products " +
                "SET IsActive = 0 " +
                "WHERE ProductId = @id";
            db.Execute(sqlQuery, id);
        }
    }
}
