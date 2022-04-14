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
    public class DialogRepository: BaseRepository, IDialogRepository
    {
        private readonly IConfiguration _configuration;
        public DialogRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Guid> Create(Dialog item)
        {
            item.DialogId = Guid.NewGuid();
            item.Date = DateTime.Now;
            var db = base.DbOpen(_configuration);
            var favorite = await db.QueryAsync<Guid>(
                "INSERT INTO \"Dialogues\" (\"DialogId\", \"User1\", \"User2\", \"Date\") " +
                "VALUES(@DialogId, @User1, @User2, @Date) " +
                "RETURNING \"DialogId\";", new { item.DialogId, item.User1, item.User2, item.Date });
            base.DbClose(db);
            return favorite.FirstOrDefault();
        }

        public async Task Delete(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "DELETE FROM \"Dialogues\" " +
                "WHERE \"DialogId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id });
            base.DbClose(db);
        }

        public async Task<IEnumerable<Dialog>> GetAll()
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\"");
            base.DbClose(db);
            return result;
        }

        public async Task<Dialog> GetById(Guid id)
        {
            var db = base.DbOpen(_configuration);
            var dialog = await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\" " +
                "WHERE \"DialogId\" = @id", new { id });
            base.DbClose(db);
            return dialog.FirstOrDefault();
        }
        public async Task<IEnumerable<Dialog>> GetByUser(Guid userId)
        {
            var db = base.DbOpen(_configuration);
            var dialog = await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\" " +
                "WHERE \"User1\" = @userId or \"User2\" = @userId", new { userId });
            base.DbClose(db);
            return dialog;
        }
        public async Task UpdateDate(Guid id)
        {
            var date = DateTime.Now;
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Dialogues\" " +
                "SET \"Date\" = @date " +
                "WHERE \"DialogId\" = @id";
            await db.ExecuteAsync(sqlQuery, new { id, date });
            base.DbClose(db);
        }
        public async Task UpdateBlocked(Guid dialogId, Guid? userId)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Dialogues\" " +
                "SET \"BlockedBy\" = @userId " +
                "WHERE \"DialogId\" = @dialogId";
            await db.ExecuteAsync(sqlQuery, new { dialogId, userId });
            base.DbClose(db);
        }
        public async Task<Dialog> CheckSimilar(Dialog dialog)
        {
            var db = base.DbOpen(_configuration);
            var result = await db.QueryAsync<Dialog>(
                "SELECT * " +
                "FROM \"Dialogues\" " +
                "WHERE (\"User1\" = @User1 and \"User2\" = @User2) or (\"User1\" = @User2 and \"User2\" = @User1)", new { dialog.User1, dialog.User2 });
            base.DbClose(db);
            return result.FirstOrDefault();
        }
    }
}
