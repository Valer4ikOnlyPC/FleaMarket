using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class DealRepository: IDealRepository
    {
        private CoreContext db;
        public DealRepository()
        {
            this.db = new CoreContext();
        }
        public IEnumerable<Deal> GetDealList()
        {
            return db.Deals.ToArray();
        }
        public IEnumerable<Deal> GetDealMasterList(User userMaster)
        {
            return db.Deals.ToArray().Where(d => d.UserMaster == userMaster.UserId);
        }
        public IEnumerable<Deal> GetDealRecipientList(User userRecipient)
        {
            return db.Deals.ToArray().Where(d => d.UserRecipient == userRecipient.UserId);
        }
        public Deal GetDeal(int id)
        {
            return db.Deals.ToArray().Where(d => d.DealId == id).First();
        }
        public void Create(Deal item)
        {
            db.Deals.Add(item);
        }
        public void Delete(int id)
        {
            db.Deals.Remove(db.Deals.ToArray().Where(d => d.DealId == id).First());
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
