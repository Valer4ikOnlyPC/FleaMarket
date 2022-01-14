using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CategoryRepository: ICategoryRepository
    {
        private CoreContext db;
        public CategoryRepository()
        {
            this.db = new CoreContext();
        }
        public IEnumerable<Category> GetCategoryList()
        {
            return db.Categorys.ToArray();
        }
        public IEnumerable<Category> GetCategoryList(Category categoryParent)
        {
            return db.Categorys.ToArray().Where(c => c.CategoryParent == categoryParent.CategoryId);
        }
        public Category GetCategory(int id)
        {
            return db.Categorys.ToArray().Where(c => c.CategoryId == id).First();
        }
        public Category GetCategoryParent(Category category)
        {
            return db.Categorys.ToArray().Where(c=> c.CategoryId==category.CategoryParent).First();
        }
        public void Create(Category item)
        {
            db.Categorys.Add(item);
        }
        public void Delete(int id)
        {
            db.Categorys.Remove(db.Categorys.ToArray().Where(c => c.CategoryId == id).First());
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
