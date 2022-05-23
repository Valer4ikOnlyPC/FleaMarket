using Domain.Core;
using Domain.Dto;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
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
        private readonly ICityService _cityService;
        private readonly IMemoryCache _cache;

        public HomeController(ICityRepository cityRepository, IRatingService ratingService, ILogger<HomeController> logger, IMemoryCache cache,
            IUserService userService, IProductService productService, IDealService dealService, ICategoryService categoryService, ICityService cityService)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _userService = userService;
            _productService = productService;
            _dealService = dealService;
            _categoryService = categoryService;
            _ratingService = ratingService;
            _cityService = cityService;
            _cache = cache;
        }
        [Authorize]
        public async Task<int> GetCountNewDeals()
        {
            var result = await _dealService.GetByRecipientCount(await _userService.GetByPhone(User.Identity.Name));
            return result;
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangeTheme()
        {
            if (!Request.Cookies.TryGetValue(Constants.CacheKey, out string? theme))
                theme = "light";
            if (theme == "light") theme = "dark";
            else theme = "light";
            Response.Cookies.Append(Constants.CacheKey, theme);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 0)
        {
            _logger.LogInformation("Open SearchPage {pageNumber}", pageNumber + 1);
            var category = await _categoryService.GetAll();
            var citySelected = (await _userService.GetByPhone(User.Identity.Name)).CityId;
            var viewProduct = await _productService.GetAll(pageNumber, citySelected);
            var allProductCount = await _productService.CountAllProduct(-1, citySelected);


            var city = await _cityService.GetAll();
            var pageCount = (int)Math.Ceiling((decimal)allProductCount / 30);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageCount = pageCount;
            if (pageCount < 1) ViewBag.PageCount = 1;
            ViewBag.IsSearch = false;
            ViewBag.City = city;
            ViewBag.CitySelected = citySelected;
            ViewBag.Category = category;
            ViewBag.CategorySelected = -1;
            ViewBag.ProductCount = viewProduct.Count();
            return View(viewProduct);
        }
        [HttpPost]
        public async Task<IActionResult> Index(SearchDto searchDto, int pageNumber = 0)
        {
            _logger.LogInformation("Search event");
            if (pageNumber != 0 ) pageNumber -= 1;
            var category = await _categoryService.GetAll();
            var city = await _cityService.GetAll();



            IEnumerable<Product> viewProduct = Enumerable.Empty<Product>();
            int allProductCount;
            if (searchDto.Search != null)
            {
                viewProduct = await _productService.GetBySearch(searchDto.Search, searchDto.CategoryId, pageNumber, searchDto.CityId);
                allProductCount = await _productService.CountBySearch(searchDto.Search, searchDto.CategoryId, searchDto.CityId);
            }
            else
            {
                viewProduct = await _productService.GetByCategory(searchDto.CategoryId, pageNumber, searchDto.CityId);
                allProductCount = await _productService.CountAllProduct(searchDto.CategoryId, searchDto.CityId);
            }

            if (allProductCount == 0) allProductCount = 1;
            var pageCount = (int)Math.Ceiling((decimal)allProductCount / 30);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageCount = pageCount;
            if (pageCount < 1) ViewBag.PageCount = 1;
            ViewBag.IsSearch = true;

            ViewBag.Category = category;
            ViewBag.CategorySelected = searchDto.CategoryId;
            ViewBag.City = city;
            ViewBag.CitySelected = searchDto.CityId;
            ViewBag.ProductCount = viewProduct.Count();
            ViewBag.SearchValue = searchDto.Search;
            return View(viewProduct);
        }
        [Authorize]
        public async Task<IActionResult> GetUserByName()
        {
            _logger.LogInformation("Opening my UserPage");
            var us = await _userService.GetByPhone(User.Identity.Name);
            return RedirectToAction("GetUser", "Home", new { userId = us.UserId });
        }
        [HttpGet]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            _logger.LogInformation("Opening UserPage by id - {userId}", userId);
            var owner = await _userService.GetById(userId);
            var city = await _cityService.GetById(owner.CityId);
            var product = (await _productService.GetByUser(owner)).Where(p => p.IsActive == Domain.Dto.ProductState.Active);
            var deal = (await _dealService.GetDealProductDtoByUser(owner)).Where(d => d.IsActive == Domain.Dto.DealState.Accepted);
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

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        
        public async Task<IActionResult> Error()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            try
            {
                var error = (ErrorModel)exceptionHandlerPathFeature.Error;
                _logger.LogError(error.Message);
                return View(error);
            }
            catch
            {
                _logger.LogError(exceptionHandlerPathFeature.Error.Message);
                var error = new ErrorModel(500, exceptionHandlerPathFeature.Error.Message);
                return View(error);
            }
        }
        public async Task<IActionResult> Error404()
        {
            throw new ErrorModel(404, "Page not found");
        }
    }
}