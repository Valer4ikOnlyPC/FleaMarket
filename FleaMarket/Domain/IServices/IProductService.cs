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
        Task<IEnumerable<Product>> GetAll(int page = 0, int cityId = -1);
        Task<IEnumerable<Product>> GetByUser(User user);
        Task<ProductPhotoDto> GetById(Guid id);
        Task<IEnumerable<Product>> GetByCategory(int categoryId, int page = 0, int cityId = -1);
        Task<int> CountAllProduct(int categoryId, int cityId = -1);
        Task<IEnumerable<Product>> GetBySearch(string search, int categoryId, int page = 0, int cityId = -1);
        Task<int> CountBySearch(string search, int categoryId, int cityId = -1);
        Task<IEnumerable<ProductPhoto>> GetPhotos(Guid productId);
        Task DeletePhoto(Guid ProductId, Guid PhotoId);
        Task UpdatePhoto(ProductDTO item);
        Task<Guid> Create(ProductDTO item);
        Task<Product> Update(Guid id, Product item);
        Task Delete(Guid id);
    }
}
