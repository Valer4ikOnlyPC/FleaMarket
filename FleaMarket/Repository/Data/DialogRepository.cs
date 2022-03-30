using Domain.Core;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data
{
    public class DialogRepository: IDialogRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _filePath;
        public DialogRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Guid> Create(Dialog item)
        {
            item.DialogId = Guid.NewGuid();
            item.Date = DateTime.Now;
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var favorite = await db.QueryAsync<Guid>(
                "INSERT INTO \"Dialogues\" (\"DialogId\", \"User1\", \"User2\", \"Date\") " +
                "VALUES(@DialogId, @User1, @User2, @Date) " +
                "RETURNING \"DialogId\";", new { item.DialogId, item.User1, item.User2, item.Date });
            return favorite.FirstOrDefault();
        }

        public async Task Delete(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "DELETE FROM \"Dialogues\" " +
                "WHERE \"DialogId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
        }

        public async Task<IEnumerable<Dialog>> GetAll()
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            return await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\"");
        }

        public async Task<Dialog> GetById(Guid id)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var dialog = await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\" " +
                "WHERE \"DialogId\" = @id", new { id });
            return dialog.FirstOrDefault();
        }
        public async Task<IEnumerable<Dialog>> GetByUser(Guid userId)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var dialog = await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\" " +
                "WHERE \"User1\" = @userId or \"User2\" = @userId", new { userId });
            return dialog;
        }
        public async Task UpdateDate(Guid id)
        {
            var date = DateTime.Now;
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "UPDATE \"Dialogues\" " +
                "SET \"Date\" = @date " +
                "WHERE \"DialogId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id, date });
        }
        public async Task<Dialog> CheckSimilar(Dialog dialog)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var result = await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\" " +
                "WHERE (\"User1\" = @User1 and \"User2\" = @User2) or (\"User1\" = @User2 and \"User2\" = @User1)", new { dialog.User1, dialog.User2 });
            return result.FirstOrDefault();
        }
    }
}
