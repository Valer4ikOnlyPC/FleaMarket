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
    public class CategoryRepository: BaseRepository, ICategoryRepository
    {
        private readonly IConfiguration _configuration;
        public CategoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<Category>> GetAll()
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\"");
            base.DbClose(db);
            return result;
        }
        public async Task<IEnumerable<Category>> GetByParent(int id)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\" " +
                "WHERE \"CategoryParent\" = @id", new { id });
            base.DbClose(db);
            return result;
        }
        public async Task<Category> GetById(int id)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\" " +
                "WHERE \"CategoryId\" = @id", new { id });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task<Category> GetParent(Category category)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Category>(
                "SELECT * " +
                "FROM \"Categories\" " +
                "WHERE \"CategoryId\" = @id", new { category.CategoryParent });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
        public async Task Create(Category item)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "INSERT INTO \"Categories\" (\"Name\", \"CategoryParent\") " +
                "VALUES(@Name, @CategoryParent)";
            await db.ExecuteAsync(sqlQuery, item);
            base.DbClose(db);
        }
        public async Task Delete(int id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "DELETE FROM \"Categories\" " +
                "WHERE \"CategoryId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }
    }
}
