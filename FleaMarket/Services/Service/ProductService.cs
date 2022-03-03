using Domain.Core;
using Domain.Dto;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;
        private readonly IDealService _dealService;
        private int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;
        public ProductService(IProductPhotoRepository productPhotoRepository, IProductRepository productRepository, IFileService fileService, IDealService dealService)
        {
            _productPhotoRepository = productPhotoRepository;
            _productRepository = productRepository;
            _fileService = fileService;
            _dealService = dealService;
        }
        public async Task<Guid> Create(ProductDTO item)
        {
            var productId = Guid.NewGuid();

            var files = await _fileService.UploadMany(item.Image, productId);
            if(files.Count() == 0)
                return Guid.Empty;
            Product product = new Product
            {
                ProductId = productId,
                Name = item.Name,
                Description = item.Description,
                CityId = item.CityId.Value,
                IsActive = ProductIsActive.Active,
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

        public async Task Delete(Guid id)
        {
            var deals = await _dealService.GetByProductId(id);
            foreach (var deal in deals)
            {
                await _dealService.Update(deal.DealId, DealIsActive.Terminated);
            }
            await _productRepository.Delete(id);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }

        public async Task<ProductPhotoDto> GetById(Guid id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
                throw new Exception("Product not found");
            var productPhoto = await _productPhotoRepository.GetByProduct(product);
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
        public async Task<IEnumerable<Product>> GetByCategory(int categoryId)
        {
            return await _productRepository.GetByCategory(categoryId);
        }

        public async Task<IEnumerable<Product>> GetByUser(User user)
        {
            if (user == null)
                throw new Exception("User is not found");
            return await _productRepository.GetByUser(user);
        }

        public async Task<Product> Update(Guid id, Product item)
        {
            return await _productRepository.Update(id, item);
        }
        public async Task<IEnumerable<Product>> GetBySearch(string search, int categoryId)
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
            var product = await _productRepository.GetBySearch(result);
            if (categoryId != -1) return product.Where(p => p.CategoryId == categoryId);
            return product;
        }
    }
}
