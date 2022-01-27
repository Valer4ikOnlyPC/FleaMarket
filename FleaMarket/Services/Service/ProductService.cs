using Domain.Core;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
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
        public ProductService(IProductPhotoRepository productPhotoRepository, IProductRepository productRepository)
        {
            _productPhotoRepository = productPhotoRepository;
            _productRepository = productRepository;
        }
        public Task<Guid> Create(ProductDTO item, string directory)
        {
            Product product = new Product
            {
                Name = item.Name,
                Description = item.Description,
                CityId = item.CityId,
                IsActive = enumIsActive.Active,
                CategoryId = item.CategoryId,
                UserId = item.UserId,
            };
            var productId = _productRepository.Create(product);

            bool flag = true;

            foreach (IFormFile img in item.Image)
            {
                if (img.Length > 0)
                {
                    string filePath = Path.Combine(directory+"\\wwwroot\\images\\" + productId.Result.ToString() + img.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(fileStream);
                        ProductPhoto productPhoto = new ProductPhoto
                        {
                            Link = "\\images\\"+ productId.Result.ToString() + img.FileName,
                            ProductId = productId.Result,
                        };
                        _productPhotoRepository.Create(productPhoto);
                    }
                    if (flag)
                    {
                        product.FirstPhoto = "\\images\\" + productId.Result.ToString() + img.FileName;
                        flag= false;
                    }
                }
            }
            _productRepository.UpdatePhoto(productId.Result, product.FirstPhoto);
            return productId;
        }

        public void Delete(Guid id)
        {
            _productRepository.Delete(id);
        }

        public IEnumerable<Product> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ProductPhotoDTO> GetById(Guid id)
        {
            Product product = await _productRepository.GetById(id);
            var productPhoto = _productPhotoRepository.GetByProduct(product);
            ProductPhotoDTO productPhotoDTO = new ProductPhotoDTO
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

        public Task<IEnumerable<Product>> GetByUser(User user)
        {
            return _productRepository.GetByUser(user);
        }

        public Task<Product> Update(Guid id, Product item)
        {
            return _productRepository.Update(id, item);
        }
    }
}
