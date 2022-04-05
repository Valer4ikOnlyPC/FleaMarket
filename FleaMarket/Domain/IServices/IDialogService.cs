using Domain.Dto;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IDialogService
    {
        Task<IEnumerable<Dialog>> GetAll();
        Task<Dialog> GetById(Guid id);
        Task<IEnumerable<DialogDto>> GetByUser(Guid userId);
        Task<IEnumerable<Message>> GetMessage(Dialog dialog);
        Task<IEnumerable<Message>> GetMessageByPage(Dialog dialog, int page = 1);
        Task<int> CountMessageByDialog(Guid dialogId);
        Task CreateMessage(Message message, Dialog dialog);
        Task<Guid> Create(Dialog item);
        Task Delete(Guid id);
        Task UpdateDate(Guid id);
        Task ReadMessage(Dialog dialog, Guid userId);
        Task<Dialog> CheckSimilar(Dialog dialog);
    }
}
