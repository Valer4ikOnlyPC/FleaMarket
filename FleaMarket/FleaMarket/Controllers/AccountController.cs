using Domain.Core;
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
        private ICityRepository _cityRepository;
        private IUserService _userService;

        public AccountController(ILogger<AccountController> logger, ICityRepository cityRepository, IUserService userService)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserDTO model)
        {
            if (ModelState.IsValid)
            {
                model.PhoneNumber = "+7" + model.PhoneNumber;
                bool result = _userService.Verification(model.PhoneNumber, model.Password);
                if (result)
                {
                    await Authenticate(model.PhoneNumber);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(_cityRepository.GetAll());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserDTO model)
        {
            if (ModelState.IsValid)
            {
                model.PhoneNumber = "+7" + model.PhoneNumber;
                var result = _userService.GetByPhone(model.PhoneNumber).Result;
                if (result == null)
                {
                    Guid userId = _userService.Create(model).Result;
                    await Authenticate(model.PhoneNumber);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(_cityRepository.GetAll());
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
            return RedirectToAction("Login", "Account");
        }
    }
}
