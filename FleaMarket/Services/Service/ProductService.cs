using Domain.Core;
using Domain.Dto;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.Product;

namespace Services.Service
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;
        private readonly IDealService _dealService;
        private readonly ICategoryService _categoryService;
        private int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;
        public ProductService(ILogger<ProductService> logger, IProductPhotoRepository productPhotoRepository, IProductRepository productRepository, IFileService fileService, IDealService dealService, ICategoryService categoryService)
        {
            _logger = logger;
            _productPhotoRepository = productPhotoRepository;
            _productRepository = productRepository;
            _fileService = fileService;
            _dealService = dealService;
            _categoryService = categoryService;
        }
        public async Task<Guid> Create(ProductDTO item)
        {
            var productId = Guid.NewGuid();

            var files = await _fileService.UploadMany(item.Image, productId, 5);
            if(files.Count() == 0)
                return Guid.Empty;
            Product product = new Product
            {
                ProductId = productId,
                Name = item.Name,
                Description = item.Description,
                CityId = item.CityId.Value,
                IsActive = ProductState.Active,
                CategoryId = item.CategoryId.Value,
                UserId = item.UserId,
                FirstPhoto = files.FirstOrDefault().Link
            };
            await _productRepository.Create(product);
            foreach (ProductPhoto photo in files)
            {
                await _productPhotoRepository.Create(photo);
            }
            return productId;
        }
        public async Task UpdatePhoto(ProductDTO item)
        {
            var productPhotoCount = (await _productPhotoRepository.GetByProduct(item.ProductId)).Count();
            var photoCount = 5 - productPhotoCount;
            var photos = await _fileService.UploadMany(item.Image, item.ProductId, photoCount);
            foreach (ProductPhoto photo in photos)
            {
                await _productPhotoRepository.Create(photo);
            }
            var images = await _productPhotoRepository.GetByProduct(item.ProductId);
            var firstPhoto = images.FirstOrDefault();
            await _productRepository.UpdatePhoto(item.ProductId, firstPhoto.Link);
        }
        public async Task Delete(Guid id)
        {
            var deals = await _dealService.GetByProductId(id);
            foreach (var deal in deals)
            {
                await _dealService.Update(deal.DealId, DealState.Terminated);
            }
            await _productRepository.Delete(id);
        }
        public async Task DeletePhoto(Guid ProductId, Guid PhotoId)
        {
            var product = await GetById(ProductId);
            if (product.Image.Count() == 0)
                return;
            var photoPath = await _productPhotoRepository.GetById(PhotoId);
            await _fileService.DeletePhoto(photoPath.Link);
            await _productPhotoRepository.Delete(PhotoId);
            var images = await _productPhotoRepository.GetByProduct(ProductId);
            var firstPhoto = images.FirstOrDefault();
            await _productRepository.UpdatePhoto(ProductId, firstPhoto.Link);
        }

        public async Task<IEnumerable<Product>> GetAll(int page = 0, int cityId = -1)
        {
            return await _productRepository.GetAll(null, page, cityId);
        }

        public async Task<ProductPhotoDto> GetById(Guid id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
                throw new ErrorModel(400, "Product not found");
            var productPhoto = await _productPhotoRepository.GetByProduct(product.ProductId);
            ProductPhotoDto productPhotoDTO = new ProductPhotoDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                FirstPhoto = product.FirstPhoto,
                Description = product.Description,
                CityId = product.CityId,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                Date = product.Date,
                UserId = product.UserId,
                Image = productPhoto.Select(p => p.Link).Where(l=>l != product.FirstPhoto)
            };
            return productPhotoDTO;
        }
        public async Task<IEnumerable<Product>> GetByCategory(int categoryId, int page = 0, int cityId = -1)
        {
            if (categoryId == -1) return await _productRepository.GetAll(null, page, cityId);
            var categories = await CategoriesToQuery(categoryId);
            return await _productRepository.GetAll(categories, page, cityId);
        }
        public async Task<int> CountAllProduct(int categoryId, int cityId = -1)
        {
            string categories = null;
            if (categoryId != -1) categories = await CategoriesToQuery(categoryId);
            return await _productRepository.GetCountAll(categories, cityId);
        }

        public async Task<IEnumerable<Product>> GetByUser(User user)
        {
            if (user == null)
                throw new ErrorModel(400, "User is not found");
            return await _productRepository.GetByUser(user);
        }

        public async Task<Product> Update(Guid id, Product item)
        {
            return await _productRepository.Update(id, item);
        }
        public async Task<IEnumerable<Product>> GetBySearch(string search, int categoryId, int page = 0, int cityId = -1)
        {
            var words = search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var resultOr = words.FirstOrDefault();
            var resultAnd = words.FirstOrDefault();
            foreach (var word in words.Skip(1))
            {
                resultOr+=string.Concat("|", word);
                resultAnd += string.Concat("&", word);
            }
            var result = string.Concat(resultOr, "|(", resultAnd, ")");

            string categories = null;
            if (categoryId != -1) categories = await CategoriesToQuery(categoryId);

            var product = await _productRepository.GetBySearch(result, categories, page, cityId);
            return product;
        }
        public async Task<int> CountBySearch(string search, int categoryId, int cityId = -1)
        {
            var words = search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var resultOr = words.FirstOrDefault();
            var resultAnd = words.FirstOrDefault();
            foreach (var word in words.Skip(1))
            {
                resultOr += string.Concat("|", word);
                resultAnd += string.Concat("&", word);
            }
            var result = string.Concat(resultOr, "|(", resultAnd, ")");

            string categories = null;
            if (categoryId != -1) categories = await CategoriesToQuery(categoryId);

            var productCount = await _productRepository.GetCountAllBySearch(result, categories, cityId);
            return productCount;
        }
        public async Task<IEnumerable<ProductPhoto>> GetPhotos(Guid productId)
        {
            return await _productPhotoRepository.GetByProduct(productId);
        }
        private async Task<string> CategoriesToQuery(int categoryId)
        {
            var categoryParent = (List<Category>)await _categoryService.GetByParent(categoryId);
            var category = await CategoriesAsync(categoryParent, new List<Category>());
            category.AddRange(categoryParent);
            var categoryResult = category.Select(x => x.CategoryId);
            string categories = "(" + categoryId.ToString();
            foreach (var catId in categoryResult)
            {
                categories = string.Concat(categories, ",", catId.ToString());
            }
            categories = string.Concat(categories, ")");
            return categories;
        }
        private async Task<List<Category>> CategoriesAsync(List<Category> categoryParent, List<Category> categoryResult)
        {
            var categories = new List<Category>();
            foreach (var categoryItem in categoryParent)
            {
                var cat = await _categoryService.GetByParent(categoryItem.CategoryId);
                categories.AddRange(cat);
            }
            categoryResult.AddRange(categories);
            if (categories.Count() == 0) return categoryResult;
            return await CategoriesAsync(categories, categoryResult);
        }
    }
}
