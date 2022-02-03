using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
        Task<IEnumerable<Category>> GetByParent(int id);
        Task<Category> GetById(int id);
        Task<Category> GetParent(Category category);
        void Create(Category item);
        void Delete(int id);
    }
}
