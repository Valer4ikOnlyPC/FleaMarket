using Dapper;
using Domain.Models;
using Npgsql;
using Domain.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data
{
    public class CategoryRepository: ICategoryRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public CategoryRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public IEnumerable<Category> GetAll()
        {
            return db.Query<Category>(
                "SELECT * " +
                "FROM Categorys").ToArray();
        }
        public IEnumerable<Category> GetByParent(Category categoryParent)
        {
            return db.Query<Category>(
                "SELECT * " +
                "FROM Categorys " +
                "WHERE CategoryParent = @CategoryId", new { categoryParent.CategoryId}).ToArray();
        }
        public Category GetById(int id)
        {
            return db.Query<Category>(
                "SELECT * " +
                "FROM Categorys " +
                "WHERE CategoryId = @id", new { id }).FirstOrDefault();
        }
        public Category GetParent(Category category)
        {
            return db.Query<Category>(
                "SELECT * " +
                "FROM Categorys " +
                "WHERE CategoryId = @id", new { category.CategoryParent }).FirstOrDefault();
        }
        public void Create(Category item)
        {
            var sqlQuery =
                "INSERT INTO Categorys " +
                "VALUES(@Name, @CategoryParent)";
            db.Execute(sqlQuery, item);
            //return item.CategoryId;
        }
        public void Delete(int id)
        {
            var sqlQuery =
                "DELETE FROM Categorys " +
                "WHERE CategoryId = @id";
            db.Execute(sqlQuery, new { id });
        }
    }
}
