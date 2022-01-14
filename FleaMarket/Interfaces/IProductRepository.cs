using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProductList();
        IEnumerable<Product> GetProductList(User user);
        Product GetProduct(int id);
        void Create(Product item);
        void Update(int id, Product item);
        void Delete(int id);
        void Save();
    }
}
