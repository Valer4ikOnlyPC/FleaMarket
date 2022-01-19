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
    public class ProductPhotoService : IProductPhotoService
    {
        private IProductPhotoRepository _photoRepository;

        public ProductPhotoService(IProductPhotoRepository repository)
        {
            _photoRepository = repository;
        }
        public Guid Create(ProductPhoto item)
        {
            return _photoRepository.Create(item);
        }

        public void Delete(Guid id)
        {
            _photoRepository.Delete(id);
        }

        public ProductPhoto GetById(Guid id)
        {
            return _photoRepository.GetById(id);
        }

        public IEnumerable<ProductPhoto> GetByProduct(Product product)
        {
            return _photoRepository.GetByProduct(product);
        }

        public ProductPhoto Update(Guid id, ProductPhoto item)
        {
            return _photoRepository.Update(id, item);
        }
    }
}
