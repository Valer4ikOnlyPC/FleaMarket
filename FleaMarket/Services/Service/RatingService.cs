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
    public class RatingService : IRatingService
    {
        IRatingRepository _ratingRepository;
        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }
        public async Task<Guid> Create(Rating item)
        {
            return await _ratingRepository.Create(item);
        }

        public async void Delete(Guid id)
        {
            _ratingRepository.Delete(id);
        }

        public async Task<IEnumerable<Rating>> GetByDeal(Deal deal)
        {
            return await _ratingRepository.GetByDeal(deal);
        }

        public async Task<IEnumerable<Rating>> GetByUser(User user)
        {
            return await _ratingRepository.GetByUser(user);
        }
    }
}
