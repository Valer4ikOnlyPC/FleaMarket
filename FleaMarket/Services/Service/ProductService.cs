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
        private int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;
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
            if(files.Count() == 0)
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
            await _productRepository.Create(product);
            foreach (ProductPhoto photo in files)
            {
                await _productPhotoRepository.Create(photo);
            }
            return productId;
        }

        public async void Delete(Guid id)
        {
            _productRepository.Delete(id);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }

        public async Task<ProductPhotoDto> GetById(Guid id)
        {
            var product = await _productRepository.GetById(id);
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
        public async Task<IEnumerable<Product>> GetByCategory(int categoryId)
        {
            return await _productRepository.GetByCategory(categoryId);
        }

        public async Task<IEnumerable<Product>> GetByUser(User user)
        {
            return await _productRepository.GetByUser(user);
        }

        public async Task<Product> Update(Guid id, Product item)
        {
            return await _productRepository.Update(id, item);
        }
        public async Task<IEnumerable<Product>> GetBySearch(string search)
        {
            var allProduct = await _productRepository.GetAll();
            return allProduct.Where(p => LevenshteinDistance(p.Name, search).GetAwaiter().GetResult()<6);
        }

        private async Task<int> LevenshteinDistance(string firstWord, string secondWord)
        {
            firstWord = firstWord.ToLower();
            secondWord = secondWord.ToLower();
            var n = firstWord.Length + 1;
            var m = secondWord.Length + 1;
            var matrixD = new int[n, m];

            const int deletionCost = 1;
            const int insertionCost = 1;

            for (var i = 0; i < n; i++)
            {
                matrixD[i, 0] = i;
            }

            for (var j = 0; j < m; j++)
            {
                matrixD[0, j] = j;
            }

            for (var i = 1; i < n; i++)
            {
                for (var j = 1; j < m; j++)
                {
                    var substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                    matrixD[i, j] = Minimum(matrixD[i - 1, j] + deletionCost,
                                            matrixD[i, j - 1] + insertionCost,
                                            matrixD[i - 1, j - 1] + substitutionCost);
                }
            }

            return matrixD[n - 1, m - 1];
        }
    }
}
