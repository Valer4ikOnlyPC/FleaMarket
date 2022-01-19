using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IProductPhotoService
    {
        IEnumerable<ProductPhoto> GetByProduct(Product product);
        ProductPhoto GetById(Guid id);
        Guid Create(ProductPhoto item);
        ProductPhoto Update(Guid id, ProductPhoto item);
        void Delete(Guid id);
    }
}
