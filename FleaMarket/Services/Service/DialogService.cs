using Domain.Core;
using Domain.Dto;
using Domain.IServices;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Service
{
    public class DialogService : IDialogService
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IUserService _userService;
        private readonly IMessageRepository _messageRepository;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = true
        };
        public DialogService(IDialogRepository dialogRepository, IUserService userService, IMessageRepository messageRepository)
        {
            _dialogRepository = dialogRepository;
            _userService = userService;
            _messageRepository = messageRepository;
        }
        public async Task<Guid> Create(Dialog item)
        {
            return await _dialogRepository.Create(item);
        }

        public async Task Delete(Guid id)
        {
            await _dialogRepository.Delete(id);
        }
        public async Task UpdateDate(Guid id)
        {
            await _dialogRepository.UpdateDate(id);
        }

        public async Task<IEnumerable<Dialog>> GetAll()
        {
            return await _dialogRepository.GetAll();
        }

        public async Task<Dialog> GetById(Guid id)
        {
            return await _dialogRepository.GetById(id);
        }

        public async Task<IEnumerable<DialogDto>> GetByUser(Guid userId)
        {
            var dialogs = await _dialogRepository.GetByUser(userId);
            var result = new List<DialogDto>();
            foreach (var dialog in dialogs)
            {
                var dialogDto = new DialogDto()
                {
                    DialogId = dialog.DialogId,
                    User1 = dialog.User1,
                    User2 = dialog.User2,
                    NameUser1 = (await _userService.GetById(dialog.User1)).Name,
                    NameUser2 = (await _userService.GetById(dialog.User2)).Name,
                    Date = dialog.Date,
                    IsRead = await CheckRead(dialog, userId)
                };
                result.Add(dialogDto);
            }
            return result;
        }

        public async Task<IEnumerable<Message>> GetMessage(Dialog dialog)
        {
            return (await _messageRepository.GetMessage(dialog.DialogId)).OrderBy(m => m.Date);
        }

        public async Task<int> CountMessageByDialog(Guid dialogId)
        {
            return await _messageRepository.CountMessageByDialog(dialogId);
        }
        public async Task<IEnumerable<Message>> GetMessageByPage(Dialog dialog, int page = 1)
        {
            return await _messageRepository.GetMessageByPage(dialog.DialogId, page);
        }

        public async Task CreateMessage(Message message, Dialog dialog)
        {
            await UpdateDate(dialog.DialogId);
            message.DialogId = dialog.DialogId;
            await _messageRepository.CreateMessage(message);
        }
        public async Task ReadMessage(Dialog dialog, Guid userId)
        {
            await _messageRepository.ReadMessage(dialog.DialogId, userId);
        }
        public async Task<Dialog> CheckSimilar(Dialog dialog)
        {
            return await _dialogRepository.CheckSimilar(dialog);
        }
        private async Task<bool> CheckRead(Dialog dialog, Guid userId)
        {
            var messages = (await GetMessageByPage(dialog)).OrderBy(m => m.Date).Where(m => m.UserId != userId);
            //var messages = (await GetMessage(dialog)).Where(m => m.UserId != userId);
            if (!messages.Any()) return true;
            return messages.LastOrDefault().IsRead;
        }
    }
}
