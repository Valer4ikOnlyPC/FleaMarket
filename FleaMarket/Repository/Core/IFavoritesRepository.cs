using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Core
{
    public interface IFavoritesRepository
    {
        IEnumerable<Favorite> GetAll();
        IEnumerable<Favorite> GetByUser(User user);
        Favorite GetById(Guid id);
        Guid Create(Favorite item);
        void Delete(Guid id);
    }
}
