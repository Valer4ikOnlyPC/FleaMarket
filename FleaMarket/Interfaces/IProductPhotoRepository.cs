using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IProductPhotoRepository
    {
        IEnumerable<ProductPhoto> GetProductPhotoList(Product product);
        ProductPhoto GetPhoto(int id);
        void Create(ProductPhoto item);
        void Update(int id, ProductPhoto item);
        void Delete(int id);
        void Save();
    }
}
