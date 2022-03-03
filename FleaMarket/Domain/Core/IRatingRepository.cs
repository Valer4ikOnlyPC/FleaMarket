using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetByUser(User user);
        Task<IEnumerable<Rating>> GetByDeal(Deal deal);
        Task<Guid> Create(Rating item);
        Task Delete(Guid id);
    }
}
