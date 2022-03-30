using Domain.Core;
using Domain.IServices;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMemoryCache _cache;

        public CityService(ICityRepository cityRepository, IMemoryCache cache)
        {
            _cityRepository = cityRepository;
            _cache = cache;
        }
        public async Task<IEnumerable<City>> GetAll()
        {
            if (!_cache.TryGetValue("allCity", out IEnumerable<City> city))
            {
                city = await _cityRepository.GetAll();
                _cache.Set("allCity", city,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            return city;
        }

        public async Task<City> GetById(int id)
        {
            if (!_cache.TryGetValue("allCity", out IEnumerable<City> allCity))
                throw new ErrorModel(400, "Cities not found");
            var result = allCity.FirstOrDefault(c => c.CityId == id);
            if (result == null)
            {
                result = await _cityRepository.GetById(id);
                _cache.Set("allCity", allCity.Append(result),
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            return result;
        }
    }
}
