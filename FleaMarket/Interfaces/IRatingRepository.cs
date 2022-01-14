using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRatingRepository
    {
        IEnumerable<Rating> GetRatingList(User user);
        void Create(Rating item);
        void Delete(int id);
        void Delete(User user);
        void Save();
    }
}
