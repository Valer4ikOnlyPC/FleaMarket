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
using Serilog;

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
            _logger.LogInformation("Open page AddProduct");
            ViewBag.City = await _cityService.GetAll();
            ViewBag.Category = await _categoryService.GetAll();
            ViewBag.CitySelected = (await _userService.GetByPhone(User.Identity.Name)).CityId;
            return PartialView();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct(ProductDTO product)
        {
            if (!(ModelState.IsValid & true)) return RedirectToAction("Index", "Home");
            _logger.LogInformation("Adding a new product");
            var city = await _cityService.GetById(product.CityId.Value);
            var category = await _categoryService.GetById(product.CategoryId.Value);
            if (category == null | city == null) return RedirectToAction("Index", "Home");
            var user = await _userService.GetByPhone(User.Identity.Name);
            product.UserId = user.UserId;
            Guid productId = await _productService.Create(product);
            _logger.LogInformation("New product id {productId}", productId);
            if (productId == Guid.Empty)
                throw new ErrorModel(400, "Photo does not match");
            return RedirectToAction("MyProducts", "Product", new { number = user.PhoneNumber });
        }

        public async Task<IActionResult> PhotoCheck(ProductDTO product)
        {
            if (product.Image == null)
                throw new ErrorModel(400, "Photo does not match");
            _logger.LogInformation("Checking added photos");
            var formFiles = product.Image;
            var filesResult = new List<IFormFile>();
            
            foreach (IFormFile file in formFiles.Take(5))
            {
                var result = await _fileService.FileCheck(file);
                if (result != 0)
                    filesResult.Add(file);
            }
            _logger.LogInformation("Count of verified Photos {filesResult}", filesResult.Count());
            return PartialView(filesResult);
        }
        public async Task<IActionResult> CategoryByParent(int CategoryId)
        {
            _logger.LogInformation("Sample category by parent {CategoryId}", CategoryId);
            var category = await _categoryService.GetByParent(CategoryId);
            return PartialView(category);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProducts(string number)
        {
            _logger.LogInformation("Open my products page by number {number}", number);
            var products = await _productService.GetByUser(await _userService.GetByPhone(number));
            var productsActive = products.Where(p => p.IsActive == ProductState.Active);
            var productsClosed = products.Where(p => (p.IsActive != ProductState.Active));
            ViewBag.ProductCount = productsActive.Count();
            ViewBag.ProductClosedCount = productsClosed.Count();
            return View(new { productsActive, productsClosed });
        }

        [HttpGet]
        public async Task<IActionResult> ViewProduct(Guid productId)
        {
            _logger.LogInformation("Open product by id {productId}", productId);
            var productPhotoDTO = await _productService.GetById(productId);
            var category = await _categoryService.GetById(productPhotoDTO.CategoryId);
            var user = await _userService.GetById(productPhotoDTO.UserId);
            int imgCount = productPhotoDTO.Image.Count();
            var city = await _cityService.GetById(productPhotoDTO.CityId);
            var similarProduct = await _productService.GetByCategory(productPhotoDTO.CategoryId);
            var dealCount = await _dealService.GetByUser(user);

            ViewBag.Master = false;
            ViewBag.Active = true;
            ViewBag.ImageCount = imgCount;
            ViewBag.Firstphoto = productPhotoDTO.FirstPhoto;
            ViewBag.Category = category.Name;
            ViewBag.User = user;
            ViewBag.City = city.Name;
            ViewBag.SimilarProduct = similarProduct.Take(45);
            ViewBag.SimilarCount = similarProduct.Count();
            ViewBag.DealCount = dealCount.Where(d => d.IsActive == DealState.Accepted).Count();
            var userMaster = await _userService.GetByPhone(User.Identity.Name);
            if (productPhotoDTO.UserId == userMaster.UserId)
                ViewBag.Master = true;
            if (productPhotoDTO.IsActive != ProductState.Active)
                ViewBag.Active = false;
            return View(productPhotoDTO);
        }
        public async Task<IActionResult> UpdatePhoto(ProductDTO product)
        {
            _logger.LogInformation("Update product photo {ProductId}", product.ProductId);
            await _productService.GetById(product.ProductId);
            await _productService.UpdatePhoto(product);
            return RedirectToAction("EditProduct", "Product", new { productId = product.ProductId });
        }
        public async Task<IActionResult> DeletePhoto(Guid ProductId, Guid PhotoId)
        {
            _logger.LogInformation("Delete photo {PhotoId} in product {ProductId}", ProductId, PhotoId);
            await _productService.DeletePhoto(ProductId, PhotoId);
            return RedirectToAction("EditProduct", "Product", new { productId = ProductId });
        }
        public async Task<IActionResult> EditProduct(Guid productId)
        {
            _logger.LogInformation("Open product edit menu by id {productId}", productId);
            var productPhotoDTO = await _productService.GetById(productId);
            int imgCount = productPhotoDTO.Image.Count();
            var photos = await _productService.GetPhotos(productId);


            ViewBag.ImageCount = imgCount;
            ViewBag.Firstphoto = productPhotoDTO.FirstPhoto;
            ViewBag.City = await _cityService.GetAll();
            ViewBag.Category = await _categoryService.GetAll();
            ViewBag.Photos = photos;

            return PartialView(productPhotoDTO);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProduct(Product product)
        {
            _logger.LogInformation("Entered data in the product editing menu {ProductId}", product.ProductId);
            var productDto = await _productService.GetById(product.ProductId);
            var user = await _userService.GetByPhone(User.Identity.Name);
            if (productDto.UserId != user.UserId) throw new ErrorModel(423, "The product does not belong to you");
            var city = await _cityService.GetById(product.CityId);
            var category = await _categoryService.GetById(product.CategoryId);
            if (category != null & city != null & ModelState.IsValid) await _productService.Update(product.ProductId, product);
            return RedirectToAction("ViewProduct", "Product", new { productId = product.ProductId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            _logger.LogInformation("Delete product by id {productId}", productId);
            var product = await _productService.GetById(productId);
            var user = await _userService.GetByPhone(User.Identity.Name);
            if(product.UserId != user.UserId) throw new ErrorModel(423,"The product does not belong to you");
            await _productService.Delete(productId);
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
            _logger.LogInformation("Adding product {productId} to favorites by user {UserId}", productId, user.UserId);
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
            _logger.LogInformation("Opening the list of favorite user {UserId}", user.UserId);
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
