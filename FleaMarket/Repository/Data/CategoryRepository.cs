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
using Microsoft.Extensions.Configuration;

namespace Repository.Data
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly IConfiguration _configuration;
        public CategoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Category>> GetAll()
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\"");
        }
        public async Task<IEnumerable<Category>> GetByParent(int id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\" " +
                "WHERE \"CategoryParent\" = @id", new { id });
            return result;
        }
        public async Task<Category> GetById(int id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\" " +
                "WHERE \"CategoryId\" = @id", new { id });
            return result.FirstOrDefault();
        }
        public async Task<Category> GetParent(Category category)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\" " +
                "WHERE \"CategoryId\" = @id", new { category.CategoryParent });
            return result.FirstOrDefault();
        }
        public async Task Create(Category item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "INSERT INTO \"Categories\" (\"Name\", \"CategoryParent\") " +
                "VALUES(@Name, @CategoryParent)";
            await db.ExecuteAsync(sqlQuery, item);
        }
        public async Task Delete(int id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM \"Categories\" " +
                "WHERE \"CategoryId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
        }
    }
}
