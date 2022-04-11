using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using Domain.Dto;

namespace Repository.Data
{
    public class BaseRepository
    {
        public IDbConnection DbOpen(IConfiguration _configuration)
        {
            return new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
        }
        public void DbClose(IDbConnection db)
        {
            db.Close();
        }
    }
}
