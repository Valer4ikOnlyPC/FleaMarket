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
    public class CityRepository: BaseRepository, ICityRepository
    {
        private readonly IConfiguration _configuration;
        public CityRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<City>> GetAll()
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<City>(
                "SELECT * " +
                "FROM \"Cities\"");
            base.DbClose(db);
            return result;
        }
        public async Task<City> GetById(int id)
        {
            var db = base.DbOpen(_configuration);
            var citys = await db.QueryAsync<City>(
                "SELECT * " +
                "FROM \"Cities\" " +
                "WHERE \"CityId\" = @id", new { id });
            base.DbClose(db);
            return citys.FirstOrDefault();
        }
        public async Task Create(City item)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "INSERT INTO \"Cities\" (\"Name\") " +
                "VALUES(@Name)";
            await db.ExecuteAsync(sqlQuery, item);
            base.DbClose(db);
        }
    }
}
