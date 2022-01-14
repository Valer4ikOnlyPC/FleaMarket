using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IFavoritesRepository
    {
        IEnumerable<Favorite> GetFavoriteList();
        IEnumerable<Favorite> GetUserFavoriteList(User user);
        Favorite GetFavorite(int id);
        void Create(Favorite item);
        void Delete(int id);
        void Save();
    }
}
