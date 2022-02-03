using Domain.Core;
using Domain.IServices;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class CategoryService : ICategoryService
    {
        ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async void Create(Category item)
        {
            _categoryRepository.Create(item);
        }

        public void Delete(int id)
        {
            _categoryRepository.Delete(id);
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<Category> GetById(int id)
        {
            return await _categoryRepository.GetById(id);
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
