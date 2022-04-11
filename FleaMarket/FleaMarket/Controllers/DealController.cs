using Domain.Dto;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Hubs;
using FleaMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static Domain.Models.Product;

namespace FleaMarket.Controllers
{
    [Authorize]
    public class DealController : Controller
    {
        private readonly ILogger<DealController> _logger;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IDealService _dealService;
        private readonly ICityService _cityService;
        private readonly IRatingService _ratingService;
        private readonly IHubContext<ChatHub> _hubContext;
        public DealController(ILogger<DealController> logger, IUserService userService, IDealService dealService,
            IProductService productService, ICityService cityService, IRatingService ratingService, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _userService = userService;
            _dealService = dealService;
            _productService = productService;
            _cityService = cityService;
            _ratingService = ratingService;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DealOffer(Guid ProductId)
        {
            _logger.LogInformation("Deal offer window opened by productId {ProductId}", ProductId);
            var product = await _productService.GetById(ProductId);
            if (product.IsActive != ProductState.Active)
                return BadRequest();
            var user = await _userService.GetByPhone(User.Identity.Name);
            var myProduct = await _productService.GetByUser(user);
            ViewBag.Product = product;
            ViewBag.MyProduct = myProduct.Where(p => p.IsActive == ProductState.Active);
            return PartialView();
        }
        [HttpGet]
        public async Task<string> CreateDeal(Guid productMaster, Guid productRecipient, Guid userRecipient)
        {
            await _productService.GetById(productMaster);
            await _productService.GetById(productRecipient);
            var user2 = await _userService.GetById(userRecipient);
            if (productMaster == productRecipient)
                productMaster = Guid.Empty;

            var user = await _userService.GetByPhone(User.Identity.Name);
            Deal deal = new Deal
            {
                ProductMaster = productMaster,
                UserMaster = user.UserId,
                ProductRecipient = productRecipient,
                UserRecipient = userRecipient
            };
            var result = await _dealService.CheckRelevant(deal);
            if (!result)
                return "Это предложение уже существует";
            var dealId = await _dealService.Create(deal);
            _logger.LogInformation("Deal created {dealId}", dealId);

            await _hubContext.Clients.User(user2.PhoneNumber).SendAsync("CountNewDeals");

            return "";
        }
        public async Task<IActionResult> MyDeal()
        {
            var user = await _userService.GetByPhone(User.Identity.Name);
            _logger.LogInformation("Opening my deals by userId {UserId}", user.UserId);
            var deal = await _dealService.GetDealProductDtoByUser(user);
            ViewBag.DealMaster = deal.Where(d => (d.IsActive == DealState.Сonsideration |
                d.IsActive == DealState.Terminated) & d.UserMaster == user.UserId).OrderByDescending(d => d.Date);
            ViewBag.DealRecipient = deal.Where(d => d.IsActive == DealState.Сonsideration & d.UserRecipient == user.UserId).OrderByDescending(d => d.Date);
            ViewBag.Deal = deal.Where(d => d.IsActive == Domain.Dto.DealState.Accepted).OrderByDescending(d => d.Date);
            ViewBag.User = user;
            return View();
        }
        public async Task<IActionResult> ViewDetails(Guid dealId)
        {
            _logger.LogInformation("Opening deal details by id {dealId}", dealId);
            var deal = await _dealService.GetById(dealId);
            if (deal == null)
                throw new ErrorModel(400, "Deal not found");
            var user = await _userService.GetByPhone(User.Identity.Name);
            var rating = await _ratingService.GetByDeal(deal);
            ViewBag.Rating = false;
            ViewBag.DealId = deal.DealId;

            var ratingResult = rating.Where(r => r.UserMasterId == user.UserId);
            if (ratingResult.Count() == 0)
                ViewBag.Rating = true;

            if (deal.UserRecipient != user.UserId)
            {
                var UserRecipient = await _userService.GetById(deal.UserRecipient);
                ViewBag.UserRecipient = UserRecipient;
                ViewBag.City = await _cityService.GetById(UserRecipient.CityId);
                return PartialView();
            }
            var userResult = await _userService.GetById(deal.UserMaster);
            ViewBag.UserRecipient = userResult;
            ViewBag.City = await _cityService.GetById(userResult.CityId);
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> ViewDetails(Guid dealId, int grade)
        {
            _logger.LogInformation("The deal is rated by id {dealId}", dealId);
            if (grade < 0 | grade > 5) return RedirectToAction("MyDeal", "Deal");
            var userMaster = await _userService.GetByPhone(User.Identity.Name);
            var deal = await _dealService.GetById(dealId);
            if (deal == null)
                throw new ErrorModel(400, "Deal not found");
            var userRecipient = deal.UserMaster;
            if (deal.UserRecipient != userMaster.UserId)
                userRecipient = deal.UserRecipient;
            var rating = new Rating()
            {
                DealId = dealId,
                Grade = grade,
                UserMasterId = userMaster.UserId,
                UserRecipientId = userRecipient
            };
            await _ratingService.Create(rating);
            return RedirectToAction("MyDeal", "Deal");
        }
        public async Task<IActionResult> Accepted(Guid dealId)
        {
            _logger.LogInformation("Deal accepted by id {dealId}", dealId);
            await _dealService.Accepted(dealId);
            var user = await _userService.GetByPhone(User.Identity.Name);
            await _hubContext.Clients.User(user.PhoneNumber).SendAsync("CountNewDeals");
            return RedirectToAction("MyDeal", "Deal");
        }
        public async Task<IActionResult> Cancel(Guid dealId)
        {
            _logger.LogInformation("Deal denied by id {dealId}", dealId);
            await _dealService.Update(dealId, DealState.Terminated);
            return RedirectToAction("MyDeal", "Deal");
        }
        public async Task<IActionResult> Delete(Guid dealId)
        {
            _logger.LogInformation("Deal deleted by id {dealId}", dealId);
            await _dealService.Delete(dealId);
            return RedirectToAction("MyDeal", "Deal");
        }
    }
}
