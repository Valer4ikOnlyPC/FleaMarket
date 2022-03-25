using Domain.Core;
using Domain.Dto;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FleaMarket.Controllers
{
    [Authorize]
    public class DialogController : Controller
    {
        private readonly ILogger<DialogController> _logger;
        private readonly IDialogService _dialogService;
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _hubContext;
        public DialogController(ILogger<DialogController> logger, IDialogService dialogService, IUserService userService, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _dialogService = dialogService;
            _userService = userService;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> AllDialog()
        {
            var user = await _userService.GetByPhone(User.Identity.Name);
            var dialogs = (await _dialogService.GetByUser(user.UserId)).OrderByDescending(x => x.Date);
            ViewBag.User = await _userService.GetByPhone(User.Identity.Name);
            return PartialView(dialogs);
        }
        public async Task<IActionResult> ViewDialog(Guid dialogId)
        {
            var dialog = await _dialogService.GetById(dialogId);
            var messages = await _dialogService.GetMessage(dialog);
            ViewBag.DialogId = dialogId;
            ViewBag.UserId = (await _userService.GetByPhone(User.Identity.Name)).UserId;
            return PartialView(messages);
        }
        public async Task ReadMessage(Guid dialogId)
        {
            var user = await _userService.GetByPhone(User.Identity.Name);
            var dialog = await _dialogService.GetById(dialogId);
            await _dialogService.ReadMessage(dialog, user.UserId);
            User user2;
            if (dialog.User1 == user.UserId)
                user2 = await _userService.GetById(dialog.User2);
            else user2 = await _userService.GetById(dialog.User1);

            await _hubContext.Clients.User(user2.PhoneNumber).SendAsync("ReadMessage", dialogId);
        }
        [HttpPost]
        public async Task<IActionResult> AddMessage(Guid dialogId, string text)
        {
            var dialog = await _dialogService.GetById(dialogId);
            var user = await _userService.GetByPhone(User.Identity.Name);
            User user2;
            if (dialog.User1 == user.UserId)
                user2 = await _userService.GetById(dialog.User2);
            else user2 = await _userService.GetById(dialog.User1);
            var message = new Message()
            {
                UserId = user.UserId,
                User = user.Name,
                Text = text,
                DateTime = DateTime.Now,
                IsRead = false
            };
            await _dialogService.CreateMessage(message, dialog);
            await _hubContext.Clients.User(user2.PhoneNumber).SendAsync("SendMessage", dialogId);

            return RedirectToAction("ViewDialog", "Dialog", new { dialogId = dialogId });
        }
        public async Task<Guid> CreateDialog(Guid userId)
        {
            var user = await _userService.GetById(userId);
            var dialog = new Dialog()
            {
                User1 = user.UserId,
                User2 = (await _userService.GetByPhone(User.Identity.Name)).UserId
            };
            var result = await _dialogService.CheckSimilar(dialog);
            if (result != null) return result.DialogId;
            var dialogId = await _dialogService.Create(dialog);
            return dialogId;
        } 
    }
}
