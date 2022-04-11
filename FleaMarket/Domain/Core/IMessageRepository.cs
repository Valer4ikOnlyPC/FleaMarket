using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessage(Guid dialogId);
        Task<IEnumerable<Message>> GetMessageByPage(Guid dialogId, int pageNumber);
        Task<int> CountMessageByDialog(Guid dialogId);
        Task CreateMessage(Message message);
        Task ReadMessage(Guid dialogId, Guid userId);
    }
}
