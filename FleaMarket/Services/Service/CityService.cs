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
    public class CityService : ICityService
    {
        ICityRepository _cityRepository;
        IProductRepository _productRepository;
        IFileService _fileService;
        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        public async Task<IEnumerable<City>> GetAll()
        {
            return await _cityRepository.GetAll();
        }

        public async Task<City> GetById(int id)
        {
            return await _cityRepository.GetById(id);
        }
    }
}
