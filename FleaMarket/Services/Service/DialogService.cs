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
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = true
        };
        public DialogService(IDialogRepository dialogRepository, IUserService userService)
        {
            _dialogRepository = dialogRepository;
            _userService = userService;
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
                    Date = dialog.Date
                };
                result.Add(dialogDto);
            }
            return result;
        }

        public async Task<IEnumerable<Message>> GetMessage(Dialog dialog)
        {
            try
            {
                var messages = JsonSerializer.Deserialize<List<Message>>(File.ReadAllText(dialog.Path));
                return messages;
            }
            catch 
            {
                return null;
            }
        }
        public async Task CreateMessage(Message message, Dialog dialog)
        {
            await UpdateDate(dialog.DialogId);
            var messages = new List<Message>();
            try
            {
                messages = JsonSerializer.Deserialize<List<Message>>(File.ReadAllText(dialog.Path));
            }
            catch
            { }
            messages.Add(message);
            File.WriteAllText(dialog.Path, JsonSerializer.Serialize<List<Message>>(messages, _options));
        }
        public async Task ReadMessage(Dialog dialog, Guid userId)
        {
            var messages = await GetMessage(dialog);
            var result = new List<Message>();
            foreach (var message in messages.Where(d => d.UserId != userId))
            {
                message.IsRead = true;
                result.Add(message);
            }
            result.AddRange(messages.Where(d => d.UserId == userId));
            var result1 = result.OrderBy(d => d.DateTime).ToList<Message>();
            File.WriteAllText(dialog.Path, JsonSerializer.Serialize<List<Message>>(result1, _options));
        }
        public async Task<Dialog> CheckSimilar(Dialog dialog)
        {
            return await _dialogRepository.CheckSimilar(dialog);
        }
    }
}
