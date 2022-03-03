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
using Domain.Dto;

namespace FleaMarket.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICityService _cityService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IDealService _dealService;
        private readonly IFavoritesService _favoritesService;
        public ProductController(ILogger<ProductController> logger, IProductService productService, IDealService dealService,
            ICityService cityService, ICategoryService categoryService, IUserService userService, IFileService fileService, IFavoritesService favoritesService)
        {
            _logger = logger;
            _productService = productService;
            _cityService = cityService;
            _categoryService = categoryService;
            _userService = userService;
            _fileService = fileService;
            _dealService = dealService;
            _favoritesService = favoritesService;
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
            if (!(ModelState.IsValid & true)) return BadRequest();
            try
            {
                if (product.CityId == null | product.CategoryId == null) return BadRequest();
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
        public async Task<IActionResult> PhotoCheck(ProductDTO product)
        {
            if (product.Image == null)
                return PartialView("Error", new ErrorViewModel { RequestId = "Photo does not match" });
            var formFiles = product.Image;
            var filesResult = new List<IFormFile>();
            foreach (IFormFile file in formFiles.Take(5))
            {
                var result = await _fileService.FileCheck(file);
                if (result != 0)
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
            try
            {
                var products = await _productService.GetByUser(await _userService.GetByPhone(number));
                var productsActive = products.Where(p => p.IsActive == ProductIsActive.Active);
                var productsClosed = products.Where(p => (p.IsActive != ProductIsActive.Active));
                ViewBag.ProductCount = productsActive.Count();
                ViewBag.ProductClosedCount = productsClosed.Count();
                return View(new { productsActive, productsClosed });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewProduct(Guid productId)
        {
            try
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
                ViewBag.DealCount = dealCount.Where(d => d.IsActive == DealIsActive.Accepted).Count();
                var userMaster = await _userService.GetByPhone(User.Identity.Name);
                if (productPhotoDTO.UserId == userMaster.UserId)
                    ViewBag.Master = true;
                if (productPhotoDTO.IsActive != ProductIsActive.Active)
                    ViewBag.Active = false;
                return View(productPhotoDTO);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> EditProduct(Guid productId)
        {
            try
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
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!(ModelState.IsValid & true)) return BadRequest();
            var result = await _productService.Update(product.ProductId, product);
            return RedirectToAction("ViewProduct", "Product", new { productId = result.ProductId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            await _productService.Delete(productId);
            var user = await _userService.GetByPhone(User.Identity.Name);
            return RedirectToAction("MyProducts", "Product", new { number = user.PhoneNumber });
        }

        [HttpGet]
        [Authorize]
        public async Task<bool> CheckFavorite(Guid productId)
        {
            var user = await _userService.GetByPhone(User.Identity.Name);
            var favorite = (await _favoritesService.GetByUser(user)).Where(f => f.ProductId == productId);
            return favorite.Any();
        }
        [Authorize]
        public async Task AddFavorite(Guid productId)
        {
            var user = await _userService.GetByPhone(User.Identity.Name);
            var result = (await _favoritesService.GetByUser(user)).Where(f => f.ProductId == productId);
            if (result.Any())
                await _favoritesService.Delete(result.FirstOrDefault().FavoriteId);
            else
            {
                var favorite = new Favorite()
                {
                    FavoriteId = Guid.NewGuid(),
                    UserId = user.UserId,
                    ProductId = productId
                };
                await _favoritesService.Create(favorite);
            }
        }
        [HttpGet]
        public async Task<IActionResult> FavoriteToList()
        {
            var user = await _userService.GetByPhone(User.Identity.Name);
            var result = await _favoritesService.GetByUser(user);
            var product = new List<ProductPhotoDto>();
            foreach (var item in result)
            {
                product.Add(await _productService.GetById(item.ProductId));
            }
            ViewBag.ProductCount = product.Count();
            return PartialView(product);
        }
    }
}
