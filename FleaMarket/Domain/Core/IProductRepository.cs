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
        Task<IEnumerable<Product>> GetAll(string categories = null, int page = 0, int cityId = -1);
        Task<int> GetCountAll(string categories = null, int cityId = -1);
        Task<IEnumerable<Product>> GetByUser(User user);
        Task<IEnumerable<Product>> GetByCategory(int categoryId);
        Task<Product> GetById(Guid id);
        Task<Guid> Create(Product item);
        Task UpdatePhoto(Guid id, string Photo);
        Task<Product> Update(Guid id, Product item);
        Task UpdateState(Guid id, int number);
        Task DealCompleted(Guid id);
        Task<IEnumerable<Product>> GetBySearch(string search, string categories = null, int page = 0, int cityId = -1);
        Task<int> GetCountAllBySearch(string search, string categories = null, int cityId = -1);
        Task Delete(Guid id);
    }
}
