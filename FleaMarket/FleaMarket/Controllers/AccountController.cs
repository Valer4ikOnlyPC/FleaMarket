using Domain.Core;
using Domain.Dto;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FleaMarket.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ICityRepository _cityRepository;
        private readonly IUserService _userService;
        private readonly string _cacheKey = "usedTheme";

        public AccountController(ILogger<AccountController> logger, ICityRepository cityRepository, IUserService userService)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            _logger.LogInformation("Authorization window opened");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                bool result = await _userService.Verification(model.PhoneNumber, model.Password);
                if (result)
                {
                    await Authenticate(model.PhoneNumber);
                    _logger.LogInformation("User is authorized");
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            _logger.LogInformation("Registration window open");
            ViewBag.City = await _cityRepository.GetAll();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.GetByPhone(model.PhoneNumber);
                if (result == null)
                {
                    var userId = await _userService.Create(model);
                    await Authenticate(model.PhoneNumber);
                    _logger.LogInformation("User registered. UserId {userId}", userId);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            ViewBag.City = await _cityRepository.GetAll();
            return View();
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User deauthorized");
            Response.Cookies.Append(_cacheKey, "light");
            return RedirectToAction("Login", "Account");
        }
    }
}
