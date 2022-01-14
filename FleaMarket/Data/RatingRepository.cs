using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class RatingRepository: IRatingRepository
    {
        private CoreContext db;
        public RatingRepository()
        {
            this.db = new CoreContext();
        }
        public IEnumerable<Rating> GetRatingList(User user)
        {
            return db.Ratings.ToArray().Where(r => r.UserId == user.UserId);
        }
        public void Create(Rating item)
        {
            db.Ratings.Add(item);
        }
        public void Delete(int id)
        {
            db.Ratings.Remove(db.Ratings.ToArray().Where(r => r.RatingId == id).First());
        }
        public void Delete(User user)
        {
            var selected = db.Ratings.ToArray().Where(r => r.UserId == user.UserId);
            foreach (Rating r in selected)
            {
                db.Ratings.Remove(r);
            }
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
