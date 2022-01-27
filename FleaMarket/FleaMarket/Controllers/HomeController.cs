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

        public HomeController(ILogger<HomeController> logger, ICityRepository cityRepository, IUserService userService)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _userService = userService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetUser()
        {
            var us = _userService.GetByPhone(User.Identity.Name).Result;
            return View(us);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}