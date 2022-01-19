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
        IEnumerable<Deal> GetAll();
        IEnumerable<Deal> GetByMaster(User userMaster);
        IEnumerable<Deal> GetByRecipient(User userRecipient);
        Deal GetById(Guid id);
        Guid Create(Deal item);
        void Delete(Guid id);
    }
}
