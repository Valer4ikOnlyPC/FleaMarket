using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IDealRepository
    {
        Task<IEnumerable<Deal>> GetAll();
        Task<IEnumerable<Deal>> GetByMaster(User userMaster);
        Task<IEnumerable<Deal>> GetByRecipient(User userRecipient);
        Task<IEnumerable<Deal>> GetByUser(User user);
        Task<IEnumerable<Deal>> GetByProduct(Guid productId);
        Task<int> GetByRecipientCount(User userRecipient);
        Task<Deal> GetById(Guid id);
        Task<Guid> Create(Deal item);
        void Update(Guid id, int number);
        void Delete(Guid id);
    }
}
