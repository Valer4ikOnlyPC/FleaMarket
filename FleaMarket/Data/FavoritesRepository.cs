using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class FavoritesRepository: IFavoritesRepository
    {
        private CoreContext db;
        public FavoritesRepository()
        {
            this.db = new CoreContext();
        }
        public IEnumerable<Favorite> GetFavoriteList()
        {
            return db.Favorites.ToArray();
        }
        public IEnumerable<Favorite> GetUserFavoriteList(User user)
        {
            return db.Favorites.ToArray().Where(f => f.UserId == user.UserId);
        }
        public Favorite GetFavorite(int id)
        {
            return db.Favorites.ToArray().Where(f => f.FavoriteId == id).First();
        }
        public void Create(Favorite item)
        {
            db.Favorites.Add(item);
        }
        public void Delete(int id)
        {
            db.Favorites.Remove(db.Favorites.ToArray().Where(f => f.FavoriteId == id).First());
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
