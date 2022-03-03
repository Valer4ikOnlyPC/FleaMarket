using Domain.Core;
using Domain.IServices;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IFavoritesRepository _favoritesRepository;
        public FavoritesService(IFavoritesRepository favoritesRepository)
        {
            _favoritesRepository = favoritesRepository;
        }
        public async Task<Guid> Create(Favorite item)
        {
            return await _favoritesRepository.Create(item);
        }

        public async Task Delete(Guid id)
        {
            await _favoritesRepository.Delete(id);
        }

        public async Task<IEnumerable<Favorite>> GetAll()
        {
            return await _favoritesRepository.GetAll();
        }

        public async Task<Favorite> GetById(Guid id)
        {
            return await _favoritesRepository.GetById(id);
        }

        public async Task<IEnumerable<Favorite>> GetByUser(User user)
        {
            return await _favoritesRepository.GetByUser(user);
        }
    }
}
