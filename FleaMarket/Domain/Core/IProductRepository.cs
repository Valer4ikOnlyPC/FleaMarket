using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetByUser(User user);
        Product GetById(Guid id);
        Guid Create(Product item);
        Product Update(Guid id, Product item);
        void Delete(Guid id);
    }
}
