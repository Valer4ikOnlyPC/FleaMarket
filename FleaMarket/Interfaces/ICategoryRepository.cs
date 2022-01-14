
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetCategoryList();
        IEnumerable<Category> GetCategoryList(Category categoryParent);
        Category GetCategory(int id);
        Category GetCategoryParent(Category category);
        void Create(Category item);
        void Delete(int id);
        void Save();
    }
}
