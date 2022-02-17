using Domain.Core;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FleaMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ICityRepository _cityRepository;
        private IUserService _userService;
        private IProductService _productService;
        private IDealService _dealService;

        public HomeController(ILogger<HomeController> logger, ICityRepository cityRepository, 
            IUserService userService, IProductService productService, IDealService dealService)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _userService = userService;
            _productService = productService;
            _dealService = dealService;
        }

        [Authorize]
        public async Task<IActionResult> Index() //добавить поиск по категории и учитывать город
        {
            //var result = await _dealService.GetByRecipientCount(await _userService.GetByPhone(User.Identity.Name));
            //ViewData["deals"] = result; 

            var allProduct = await _productService.GetAll();
            ViewBag.ProductCount = allProduct.Count();
            return View(allProduct);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string Search)
        {
            if(!ModelState.IsValid)
                return View();
            var allProduct = await _productService.GetBySearch(Search);
            ViewBag.ProductCount = allProduct.Count();
            ViewBag.SearchValue = Search;
            return View(allProduct);
        }
        [Authorize]
        public async Task<IActionResult> GetUserByName()
        {
            var us = await _userService.GetByPhone(User.Identity.Name);
            ViewBag.User = us;
            return RedirectToAction("GetUser", "Home", new { userId = us.UserId });
        }
        [HttpGet]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var us = await _userService.GetById(userId);
            ViewBag.User = us;
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}