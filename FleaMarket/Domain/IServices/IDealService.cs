using Domain.Dto;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IDealService
    {
        Task<IEnumerable<Deal>> GetAll();
        Task<IEnumerable<DealDto>> GetByMaster(User userMaster);
        Task<IEnumerable<DealDto>> GetByRecipient(User userRecipient);
        Task<IEnumerable<DealDto>> GetByUser(User user);
        Task<IEnumerable<Deal>> GetByProductId(Guid productId);
        Task<IEnumerable<DealProductDto>> GetDealProductDtoByUser(User user);
        Task<int> GetByRecipientCount(User userRecipient);
        Task<bool> CheckRelevant(Deal deal);
        Task Accepted(Guid dealId);
        Task Update(Guid dealId, DealState enumIsActive);
        Task<Deal> GetById(Guid id);
        Task<Guid> Create(Deal item);
        Task Delete(Guid id);
    }
}
