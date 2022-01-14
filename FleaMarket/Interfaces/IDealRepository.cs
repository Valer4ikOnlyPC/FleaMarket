using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IDealRepository
    {
        IEnumerable<Deal> GetDealList();
        IEnumerable<Deal> GetDealMasterList(User userMaster);
        IEnumerable<Deal> GetDealRecipientList(User userRecipient);
        Deal GetDeal(int id);
        void Create(Deal item);
        void Delete(int id);
        void Save();
    }
}
