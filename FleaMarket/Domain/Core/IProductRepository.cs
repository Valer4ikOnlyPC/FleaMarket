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
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetByUser(User user);
        Task<Product> GetById(Guid id);
        Task<Guid> Create(Product item);
        void UpdatePhoto(Guid id, string Photo);
        Task<Product> Update(Guid id, Product item);
        void DealCompleted(Guid id);
        void Delete(Guid id);
    }
}
