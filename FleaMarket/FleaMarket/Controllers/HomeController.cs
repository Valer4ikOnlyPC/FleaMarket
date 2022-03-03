using Domain.Core;
using Domain.Dto;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FleaMarket.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICityRepository _cityRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IDealService _dealService;
        private readonly ICategoryService _categoryService;
        private readonly IRatingService _ratingService;

        public HomeController(ILogger<HomeController> logger, ICityRepository cityRepository, IRatingService ratingService,
            IUserService userService, IProductService productService, IDealService dealService, ICategoryService categoryService)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _userService = userService;
            _productService = productService;
            _dealService = dealService;
            _categoryService = categoryService;
            _ratingService = ratingService;
        }
        [Authorize]
        public async Task<int> GetCountNewDeals()
        {
            var result = await _dealService.GetByRecipientCount(await _userService.GetByPhone(User.Identity.Name));
            return result;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var category = await _categoryService.GetAll();
            var allProduct = await _productService.GetAll();
            var city = await _cityRepository.GetAll();
            ViewBag.City = city;
            ViewBag.CitySelected = (await _userService.GetByPhone(User.Identity.Name)).CityId;
            ViewBag.Category = category;
            ViewBag.CategorySelected = -1;
            ViewBag.ProductCount = allProduct.Count();
            return View(allProduct);
        }
        [HttpPost]
        public async Task<IActionResult> Index(SearchDto searchDto)
        {
            if (!ModelState.IsValid & searchDto.CategoryId == -1)
                return RedirectToAction("Index", "Home");
            var category = await _categoryService.GetAll();
            var city = await _cityRepository.GetAll();
            IEnumerable<Product> allProduct = new List<Product>();
            if(searchDto.Search != null)
                allProduct = await _productService.GetBySearch(searchDto.Search, searchDto.CategoryId);
            else
                allProduct = await _productService.GetByCategory(searchDto.CategoryId);
            if(searchDto.CityId != -1) allProduct = allProduct.Where(x => x.CityId== searchDto.CityId);
            ViewBag.Category = category;
            ViewBag.CategorySelected = searchDto.CategoryId;
            ViewBag.City = city;
            ViewBag.CitySelected = searchDto.CityId;
            ViewBag.ProductCount = allProduct.Count();
            ViewBag.SearchValue = searchDto.Search;
            return View(allProduct);
        }
        [Authorize]
        public async Task<IActionResult> GetUserByName()
        {
            var us = await _userService.GetByPhone(User.Identity.Name);
            return RedirectToAction("GetUser", "Home", new { userId = us.UserId });
        }
        [HttpGet]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            try
            {
                var owner = await _userService.GetById(userId);
                var city = await _cityRepository.GetById(owner.CityId);
                var product = (await _productService.GetByUser(owner)).Where(p => p.IsActive == Domain.Dto.ProductIsActive.Active);
                var deal = (await _dealService.GetDealProductDtoByUser(owner)).Where(d => d.IsActive == Domain.Dto.DealIsActive.Accepted);
                var user = await _userService.GetByPhone(User.Identity.Name);
                var ratingCount = (await _ratingService.GetByUser(owner)).Count();
                var favorite = false;
                if (owner.UserId == user.UserId)
                    favorite = true;
                ViewBag.MyProfile = favorite;
                ViewBag.RatingCount = ratingCount;
                ViewBag.Deal = deal.OrderByDescending(d => d.Date);
                ViewBag.Product = product;
                ViewBag.User = owner;
                ViewBag.City = city;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        
        public async Task<IActionResult> Error(string errorMessage)
        {
            return View(new ErrorViewModel { RequestId = errorMessage });
        }
    }
}