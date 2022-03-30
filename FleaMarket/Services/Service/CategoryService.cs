using Domain.Core;
using Domain.IServices;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _cache;
        public CategoryService(ILogger<CategoryService> logger, ICategoryRepository categoryRepository, IMemoryCache cache)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _cache = cache;
        }
        public async Task Create(Category item)
        {
            await _categoryRepository.Create(item);
        }

        public async Task Delete(int id)
        {
            await _categoryRepository.Delete(id);
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            if (!_cache.TryGetValue("allCategory", out IEnumerable<Category> category))
            {
                category = await _categoryRepository.GetAll();
                _cache.Set("allCategory", category,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            return category;
        }

        public async Task<Category> GetById(int id)
        {
            if (!_cache.TryGetValue("allCategory", out IEnumerable<Category> allCategory))
                throw new ErrorModel(400, "Categories not found");
            var result = allCategory.FirstOrDefault(c => c.CategoryId == id);
            if (result == null)
            {
                result = await _categoryRepository.GetById(id);
                _cache.Set("allCategory", allCategory.Append(result),
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            return result;
        }

        public async Task<IEnumerable<Category>> GetByParent(int id)
        {
            return await _categoryRepository.GetByParent(id);
        }

        public async Task<Category> GetParent(Category category)
        {
            return await _categoryRepository.GetParent(category);
        }
    }
}
