using Domain.DTO;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Task<IEnumerable<Product>> GetByUser(User user);
        Task<ProductPhotoDTO> GetById(Guid id);
        Task<Guid> Create(ProductDTO item, string directory);
        Task<Product> Update(Guid id, Product item);
        void Delete(Guid id);
    }
}
