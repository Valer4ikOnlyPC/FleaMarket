using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Core
{
    public interface IRatingRepository
    {
        IEnumerable<Rating> GetByUser(User user);
        Guid Create(Rating item);
        void Delete(Guid id);
    }
}
