using Dapper;
using Domain.Core;
using Domain.DTO;
using Domain.IServices;
using Domain.Models;
using FleaMarket.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using Repository.Data;
using Services.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class ProductTests
    {
        private IProductService _productService;

        public ProductTests()
        {
            var mockLogger = new Mock<ILogger<ProductService>>();
            var mockProductPhoto = new Mock<IProductPhotoRepository>();
            var mockProduct = new Mock<IProductRepository>();
            var mockFile = new Mock<IFileService>();
            var mockDeal = new Mock<IDealService>();
            var categories = new List<Category>()
            {
                new Category() { CategoryId = 1, Name = "Test1", CategoryParent = -1 },
                new Category() { CategoryId = 2, Name = "Test2", CategoryParent = 1 },
                new Category() { CategoryId = 3, Name = "Test3", CategoryParent = 1 },
                new Category() { CategoryId = 4, Name = "Test4", CategoryParent = 1 },
                new Category() { CategoryId = 5, Name = "Test5", CategoryParent = 2 },
                new Category() { CategoryId = 6, Name = "Test6", CategoryParent = 2 },
                new Category() { CategoryId = 7, Name = "Test7", CategoryParent = 2 },
                new Category() { CategoryId = 8, Name = "Test8", CategoryParent = 3 },
                new Category() { CategoryId = 9, Name = "Test9", CategoryParent = 3 },
                new Category() { CategoryId = 10, Name = "Test10", CategoryParent = 4 },
                new Category() { CategoryId = 11, Name = "Test11", CategoryParent = 10 },
                new Category() { CategoryId = 12, Name = "Test12", CategoryParent = 10 },
                new Category() { CategoryId = 13, Name = "Test13", CategoryParent = 10 },
            };
            var mockCategory = new Mock<ICategoryService>();
            mockCategory.Setup(c => c.GetByParent(1)).ReturnsAsync(categories.Where(c => c.CategoryParent == 1).ToList());
            mockCategory.Setup(c => c.GetByParent(2)).ReturnsAsync(categories.Where(c => c.CategoryParent == 2).ToList());
            mockCategory.Setup(c => c.GetByParent(3)).ReturnsAsync(categories.Where(c => c.CategoryParent == 3).ToList());
            mockCategory.Setup(c => c.GetByParent(4)).ReturnsAsync(categories.Where(c => c.CategoryParent == 4).ToList());
            mockCategory.Setup(c => c.GetByParent(10)).ReturnsAsync(categories.Where(c => c.CategoryParent == 10).ToList());
            mockCategory.Setup(c => c.GetByParent(13)).ReturnsAsync(new List<Category>());

            _productService = new ProductService(mockLogger.Object, mockProductPhoto.Object, mockProduct.Object, mockFile.Object, mockDeal.Object, mockCategory.Object);
        }
        [Fact]
        public async Task CategoriesToQueryTest1()
        {
            //Arrange
            var arrange = "(1,5,6,7,8,9,10,11,12,13,2,3,4)";
            //Act
            var result = await _productService.CategoriesToQuery(1);
            //Assert
            Assert.Equal(arrange, result);
        }
        [Fact]
        public async Task CategoriesToQueryTest2()
        {
            //Arrange
            var arrange = "(10,11,12,13)";
            //Act
            var result = await _productService.CategoriesToQuery(10);
            //Assert
            Assert.Equal(arrange, result);
        }
        [Fact]
        public async Task CategoriesToQueryTest3()
        {
            //Arrange
            var arrange = "(13)";
            //Act
            var result = await _productService.CategoriesToQuery(13);
            //Assert
            Assert.Equal(arrange, result);
        }
    }
}
