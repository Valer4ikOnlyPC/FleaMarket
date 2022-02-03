using Domain.Core;
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
        IProductPhotoRepository _productPhotoRepository;
        IProductRepository _productRepository;
        IFileService _fileService;
        public ProductService(IProductPhotoRepository productPhotoRepository, IProductRepository productRepository, IFileService fileService)
        {
            _productPhotoRepository = productPhotoRepository;
            _productRepository = productRepository;
            _fileService = fileService;
        }
        public async Task<Guid> Create(ProductDTO item)
        {
            var productId = Guid.NewGuid();

            var files = await _fileService.UploadMany(item.Image, productId);
            if(files.Count()==0)
                return Guid.Empty;
            Product product = new Product
            {
                ProductId = productId,
                Name = item.Name,
                Description = item.Description,
                CityId = item.CityId,
                IsActive = enumIsActive.Active,
                CategoryId = item.CategoryId,
                UserId = item.UserId,
                FirstPhoto = files.FirstOrDefault().Link
            };
            _productRepository.Create(product);
            foreach (ProductPhoto photo in files)
            {
                _productPhotoRepository.Create(photo);
            }
            return productId;

        }

        public async void Delete(Guid id)
        {
            _productRepository.Delete(id);
        }

        public IEnumerable<Product> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ProductPhotoDto> GetById(Guid id)
        {
            Product product = await _productRepository.GetById(id);
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
                UserId = product.UserId,
                Image = productPhoto.Select(p => p.Link).Where(l=>l != product.FirstPhoto)
            };
            return productPhotoDTO;
        }

        public async Task<IEnumerable<Product>> GetByUser(User user)
        {
            return await _productRepository.GetByUser(user);
        }

        public async Task<Product> Update(Guid id, Product item)
        {
            return await _productRepository.Update(id, item);
        }
    }
}
