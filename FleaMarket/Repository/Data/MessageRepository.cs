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
    public class MessageRepository : BaseRepository, IMessageRepository
    {
        private readonly IConfiguration _configuration;
        public MessageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task CreateMessage(Message message)
        {
            var db = base.DbOpen(_configuration);
            var favorite = await db.QueryAsync<Guid>(
                "INSERT INTO \"Messages\" (\"UserId\", \"User\", \"Text\", \"Date\", \"IsRead\", \"DialogId\") " +
                "VALUES(@UserId, @User, @Text, @Date, @IsRead, @DialogId) " +
                "RETURNING \"DialogId\";", new { message.UserId, message.User, message.Text, message.Date, message.IsRead, message.DialogId });
            base.DbClose(db);
        }

        public async Task<IEnumerable<Message>> GetMessage(Guid dialogId)
        {
            var db = base.DbOpen(_configuration);
            var messages = await db.QueryAsync<Message>(
                "SELECT * " +
                "FROM \"Messages\" " +
                "WHERE \"DialogId\" = @dialogId ", new { dialogId });
            base.DbClose(db);
            return messages;
        }
        public async Task<int> CountMessageByDialog(Guid dialogId)
        {
            var db = base.DbOpen(_configuration);
            var messages = await db.QueryAsync<int>(
                "SELECT COUNT(*) " +
                "FROM \"Messages\" " +
                "WHERE \"DialogId\" = @dialogId ", new { dialogId });
            base.DbClose(db);
            return messages.FirstOrDefault();
        }
        public async Task<IEnumerable<Message>> GetMessageByPage(Guid dialogId, int pageNumber)
        {
            var db = base.DbOpen(_configuration);
            var messages = await db.QueryAsync<Message>(
                "SELECT * " +
                "FROM \"Messages\" " +
                "WHERE \"DialogId\" = @dialogId " +
                "ORDER BY \"Date\" desc " +
                "LIMIT 30*@pageNumber " , new { dialogId, pageNumber });
            base.DbClose(db);
            return messages;
        }

        public async Task ReadMessage(Guid dialogId, Guid userId)
        {
            var db = base.DbOpen(_configuration);
            var sqlQuery =
                "UPDATE \"Messages\" " +
                "SET \"IsRead\" = true " +
                "WHERE ( \"DialogId\" = @dialogId ) and ( \"UserId\" != @userId )";
            await db.ExecuteAsync(sqlQuery, new { dialogId, userId });
            base.DbClose(db);
        }
    }
}
