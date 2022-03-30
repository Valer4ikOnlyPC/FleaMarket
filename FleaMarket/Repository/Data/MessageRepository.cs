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
    public class MessageRepository : IMessageRepository
    {
        private readonly IConfiguration _configuration;
        public MessageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task CreateMessage(Message message)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var favorite = await db.QueryAsync<Guid>(
                "INSERT INTO \"Messages\" (\"UserId\", \"User\", \"Text\", \"Date\", \"IsRead\", \"DialogId\") " +
                "VALUES(@UserId, @User, @Text, @Date, @IsRead, @DialogId) " +
                "RETURNING \"DialogId\";", new { message.UserId, message.User, message.Text, message.Date, message.IsRead, message.DialogId });
        }

        public async Task<IEnumerable<Message>> GetMessage(Guid dialogId)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var messages = await db.QueryAsync<Message>(
                "SELECT * " +
                "FROM \"Messages\" " +
                "WHERE \"DialogId\" = @dialogId", new { dialogId });
            return messages;
        }

        public async Task ReadMessage(Guid dialogId, Guid userId)
        {
            IDbConnection db = new NpgsqlConnection(_configuration.GetConnectionString("myconn"));
            var sqlQuery =
                "UPDATE \"Messages\" " +
                "SET \"IsRead\" = true " +
                "WHERE ( \"DialogId\" = @dialogId ) and ( \"UserId\" != @userId )";
            await db.ExecuteAsync(sqlQuery, new { dialogId, userId });
        }
    }
}
