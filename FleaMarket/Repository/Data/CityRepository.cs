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
    public class CityRepository: ICityRepository
    {
        private readonly IConfiguration _configuration;
        public CityRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<City>> GetAll()
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<City>(
                "SELECT * " +
                "FROM \"Cities\"");
        }
        public async Task<City> GetById(int id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var citys = await db.QueryAsync<City>(
                "SELECT * " +
                "FROM \"Cities\" " +
                "WHERE \"CityId\" = @id", new { id });
            return citys.FirstOrDefault();
        }
        public async Task Create(City item)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "INSERT INTO \"Cities\" (\"Name\") " +
                "VALUES(@Name)";
            await db.ExecuteAsync(sqlQuery, item);
        }
    }
}
