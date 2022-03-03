using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IProductPhotoRepository
    {
        Task<IEnumerable<ProductPhoto>> GetByProduct(Product product);
        Task<ProductPhoto> GetById(Guid id);
        Task<Guid> Create(ProductPhoto item);
        Task<ProductPhoto> Update(Guid id, ProductPhoto item);
        Task Delete(Guid id);
    }
}
