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
        public IEnumerable<City> GetAll()
        {
            string constr = _configuration.GetConnectionString("myconn");
            IDbConnection db = new NpgsqlConnection(constr);
            return db.Query<City>(
                "SELECT * " +
                "FROM Citys").ToArray();
        }
        public async Task<City> GetById(int id)
        {
            string constr = _configuration.GetConnectionString("myconn");
            IDbConnection db = new NpgsqlConnection(constr);
            var citys = await db.QueryAsync<City>(
                "SELECT * " +
                "FROM Citys " +
                "WHERE CityId = @id", new { id });
            return citys.FirstOrDefault();
        }
    }
}
