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
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetByUser(User user);
        Task<ProductPhotoDto> GetById(Guid id);
        Task<IEnumerable<Product>> GetByCategory(int categoryId);
        Task<IEnumerable<Product>> GetBySearch(string search);
        Task<Guid> Create(ProductDTO item);
        Task<Product> Update(Guid id, Product item);
        void Delete(Guid id);
    }
}
