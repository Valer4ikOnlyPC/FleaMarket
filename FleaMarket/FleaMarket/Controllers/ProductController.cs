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
using Microsoft.AspNetCore.Http;

namespace FleaMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private ICityService _cityService;
        private IUserService _userService;
        private IFileService _fileService;
        private IDealService _dealService;
        public ProductController(ILogger<ProductController> logger, IProductService productService, IDealService dealService,
            ICityService cityService, ICategoryService categoryService, IUserService userService, IFileService fileService)
        {
            _logger = logger;
            _productService = productService;
            _cityService = cityService;
            _categoryService = categoryService;
            _userService = userService;
            _fileService = fileService;
            _dealService = dealService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.City = await _cityService.GetAll();
            ViewBag.Category = await _categoryService.GetAll();
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct(ProductDTO product)
        {
            if (ModelState.IsValid & product.Image != null)
            {
                try
                {
                    var user = await _userService.GetByPhone(User.Identity.Name);
                    product.UserId = user.UserId;
                    Guid productId = await _productService.Create(product);
                    if (productId == Guid.Empty)
                        return BadRequest();
                    return RedirectToAction("MyProducts", "Product", new { number = user.PhoneNumber });
                }
                catch (Exception)
                {
                    return BadRequest();
                }

            }
            return BadRequest();
        }
        public async Task<IActionResult> PhotoCheck(ProductDTO product)
        {
            if (product.Image == null)
                return BadRequest();
            var formFiles = product.Image;
            var filesResult = new List<IFormFile>();
            foreach (IFormFile file in formFiles.Take(5))
            {
                var result = _fileService.FileCheck(file);
                if(result != 2)
                    filesResult.Add(file);
            }
            return PartialView(filesResult);
        }
        public async Task<IActionResult> CategoryByParent(int CategoryId)
        {
            var category = await _categoryService.GetByParent(CategoryId);
            return PartialView(category);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProducts(string number)
        {
            var products = await _productService.GetByUser( await _userService.GetByPhone(number));
            var result = products.Where(p => p.IsActive == enumIsActive.Active);
            ViewBag.ProductCount = result.Count();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewProduct(Guid productId)
        {
            var productPhotoDTO = await _productService.GetById(productId);
            var categoty = await _categoryService.GetById(productPhotoDTO.CategoryId);
            var user = await _userService.GetById(productPhotoDTO.UserId);
            int imgCount = productPhotoDTO.Image.Count();
            var city = await _cityService.GetById(productPhotoDTO.CityId);
            var similarProduct = await _productService.GetByCategory(productPhotoDTO.CategoryId);
            var dealCount = await _dealService.GetByUser(user);

            ViewBag.Master = false;
            ViewBag.Active = true;
            ViewBag.ImageCount = imgCount;
            ViewBag.Firstphoto = productPhotoDTO.FirstPhoto;
            ViewBag.Category = categoty.Name;
            ViewBag.User = user;
            ViewBag.City = city.Name;
            ViewBag.SimilarProduct = similarProduct.Take(45);
            ViewBag.SimilarCount = similarProduct.Count();
            ViewBag.DealCount = dealCount.Where(d => d.IsActive == Deal.enumIsActive.Accepted).Count();
            var userMaster = await _userService.GetByPhone(User.Identity.Name);
            if (productPhotoDTO.UserId == userMaster.UserId )
                ViewBag.Master = true;
            if (productPhotoDTO.IsActive != enumIsActive.Active)
                ViewBag.Active = false;
            return View(productPhotoDTO);
        }

        public async Task<IActionResult> EditProduct(Guid productId)
        {
            var productPhotoDTO = await _productService.GetById(productId);
            var categoty = await _categoryService.GetById(productPhotoDTO.CategoryId);
            var user = await _userService.GetById(productPhotoDTO.UserId);
            int imgCount = productPhotoDTO.Image.Count();
            var city = await _cityService.GetById(productPhotoDTO.CityId);

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
