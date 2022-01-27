using Domain.Core;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Repository.Data;
using System.Diagnostics;
using System.Security.Claims;
using static Domain.Models.Product;

namespace FleaMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private IProductService _productService;
        private ICityRepository _cityRepository;
        private ICategoryRepository _categoryRepository;
        private IUserService _userService;
        public ProductController(ILogger<ProductController> logger, IProductService productService,
            ICityRepository cityRepository, ICategoryRepository categoryRepository, IUserService userService)
        {
            _logger = logger;
            _productService = productService;
            _cityRepository = cityRepository;
            _categoryRepository = categoryRepository;
            _userService = userService;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult AddProduct()
        {
            ViewBag.City = _cityRepository.GetAll();
            ViewBag.Category = _categoryRepository.GetByParent(-1).Result;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDTO product)
        {
            if (ModelState.IsValid & product.Image != null)
            {
                try
                {
                    var user = await _userService.GetByPhone(User.Identity.Name);
                    product.UserId = user.UserId;
                    Guid productId = await _productService.Create(product, System.IO.Directory.GetCurrentDirectory());
                    return RedirectToAction("MyProducts", "Product", new { number = user.PhoneNumber });
                }
                catch (Exception)
                {
                    return BadRequest();
                }

            }
            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProducts(string number)
        {
            var products = await _productService.GetByUser( await _userService.GetByPhone(number));
            var result = products.Where(p => (int)p.IsActive == 2);
            ViewBag.ProductCount = result.Count();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewProduct(Guid productId)
        {
            var productPhotoDTO = await _productService.GetById(productId);
            var categoty = await _categoryRepository.GetById(productPhotoDTO.CategoryId);
            var user = await _userService.GetById(productPhotoDTO.UserId);
            int imgCount = productPhotoDTO.Image.Count();
            var city = await _cityRepository.GetById(productPhotoDTO.CityId);

            ViewBag.Master = false;
            ViewBag.ImageCount = imgCount;
            ViewBag.Firstphoto = productPhotoDTO.FirstPhoto;
            ViewBag.Category = categoty.Name;
            ViewBag.User = user;
            ViewBag.City = city.Name;
            var userMaster = await _userService.GetByPhone(User.Identity.Name);
            if (productPhotoDTO.UserId== userMaster.UserId)
                ViewBag.Master = true;
            return View(productPhotoDTO);
        }

        public async Task<IActionResult> EditProduct(Guid productId)
        {
            var productPhotoDTO = await _productService.GetById(productId);
            var categoty = await _categoryRepository.GetById(productPhotoDTO.CategoryId);
            var user = await _userService.GetById(productPhotoDTO.UserId);
            int imgCount = productPhotoDTO.Image.Count();
            var city = await _cityRepository.GetById(productPhotoDTO.CityId);

            ViewBag.ImageCount = imgCount;
            ViewBag.Firstphoto = productPhotoDTO.FirstPhoto;
            ViewBag.Category = categoty.Name;
            ViewBag.User = user;
            ViewBag.City = city.Name;

            return View(productPhotoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            var result = await _productService.Update(product.ProductId, product);
            return RedirectToAction("ViewProduct", "Product", new { productId = result.ProductId });
        }

        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            _productService.Delete(productId);
            var user = await _userService.GetByPhone(User.Identity.Name);
            return RedirectToAction("MyProducts", "Product", new { number = user.PhoneNumber });
        }
    }
}
