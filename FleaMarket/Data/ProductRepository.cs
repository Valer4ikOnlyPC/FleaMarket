using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ProductRepository: IProductRepository
    {
        private CoreContext db;
        public ProductRepository()
        {
            this.db = new CoreContext();
        }
        public IEnumerable<Product> GetProductList()
        {
            return db.Products.ToArray();
        }
        public IEnumerable<Product> GetProductList(User user)
        {
            return db.Products.ToArray().Where(p => p.UserId == user.UserId);
        }
        public Product GetProduct(int id)
        {
            return db.Products.ToArray().Where(p => p.ProductId == id).First();
        }
        public void Create(Product item)
        {
            db.Products.Add(item);
        }
        public void Update(int id, Product item)
        {
            Product selectedProduct = db.Products.ToArray().Where(p => p.ProductId == id).First();
            selectedProduct = item;
            Save();
        }
        public void Delete(int id)
        {
            db.Products.Remove(db.Products.ToArray().Where(p => p.ProductId == id).First());
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
