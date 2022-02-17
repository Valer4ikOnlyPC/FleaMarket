using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IRatingService
    {
        Task<IEnumerable<Rating>> GetByUser(User user);
        Task<IEnumerable<Rating>> GetByDeal(Deal deal);
        Task<Guid> Create(Rating item);
        void Delete(Guid id);
    }
}
