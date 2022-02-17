using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Domain.Models.Product;

namespace FleaMarket.Controllers
{
    [Authorize]
    public class DealController : Controller
    {
        private readonly ILogger<DealController> _logger;
        private IUserService _userService;
        private IProductService _productService;
        private IDealService _dealService;
        private ICityService _cityService;
        private IRatingService _ratingService;
        public DealController(ILogger<DealController> logger, IUserService userService, IDealService dealService,
            IProductService productService, ICityService cityService, IRatingService ratingService)
        {
            _logger = logger;
            _userService = userService;
            _dealService = dealService;
            _productService = productService;
            _cityService = cityService;
            _ratingService = ratingService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DealOffer(Guid ProductId)
        {
            var product = await _productService.GetById(ProductId);
            if (product.IsActive != Product.enumIsActive.Active)
                return BadRequest();
            var user = await _userService.GetByPhone(User.Identity.Name);
            var myProduct = await _productService.GetByUser(user);
            ViewBag.Product = product;
            ViewBag.MyProduct = myProduct.Where(p => p.IsActive == enumIsActive.Active);
            return PartialView();
        }
        [HttpGet]
        public async Task<string> CreateDeal(Guid productMaster, Guid productRecipient, Guid userRecipient)
        {
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
            return "";
        }
        public async Task<IActionResult> MyDeal()
        {
            var user = await _userService.GetByPhone(User.Identity.Name);
            var allDeal = await _dealService.GetByUser(user);
            ViewBag.DealMaster = allDeal.Where(d => (d.IsActive == Deal.enumIsActive.Сonsideration |
                d.IsActive == Deal.enumIsActive.Terminated) & d.UserMaster == user.UserId);
            ViewBag.DealRecipient = allDeal.Where(d => d.IsActive == Deal.enumIsActive.Сonsideration & d.UserRecipient == user.UserId);
            ViewBag.Deal = allDeal.Where(d => d.IsActive == Deal.enumIsActive.Accepted);
            return View();
        }
        public async Task<IActionResult> ViewDetails(Guid dealId)
        {
            var deal = await _dealService.GetById(dealId);
            var user = await _userService.GetByPhone(User.Identity.Name);
            var rating = await _ratingService.GetByDeal(deal);
            ViewBag.Rating = false;
            ViewBag.DealId = deal.DealId;

            var rating_result = rating.Where(r => r.UserMasterId == user.UserId);
            if(rating_result.Count() == 0)
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
            if(grade<0|grade>5) return RedirectToAction("MyDeal", "Deal");
            var userMaster = await _userService.GetByPhone(User.Identity.Name);
            var deal = await _dealService.GetById(dealId);
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
            _dealService.Accepted(dealId);
            return RedirectToAction("MyDeal", "Deal");
        }
        public async Task<IActionResult> Cancel(Guid dealId)
        {
            _dealService.Update(dealId, Deal.enumIsActive.Terminated);
            return RedirectToAction("MyDeal", "Deal");
        }
        public async Task<IActionResult> Delete(Guid dealId)
        {
            _dealService.Delete(dealId);
            return RedirectToAction("MyDeal", "Deal");
        }
    }
}
