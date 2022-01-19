using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        IEnumerable<Category> GetByParent(Category categoryParent);
        Category GetById(int id);
        Category GetParent(Category category);
        void Create(Category item);
        void Delete(int id);
    }
}
