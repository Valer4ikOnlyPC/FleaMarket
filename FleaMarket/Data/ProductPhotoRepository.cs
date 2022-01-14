using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ProductPhotoRepository: IProductPhotoRepository
    {
        private CoreContext db;
        public ProductPhotoRepository()
        {
            this.db = new CoreContext();
        }
        public IEnumerable<ProductPhoto> GetProductPhotoList(Product product)
        {
            return db.ProductPhotos.ToArray().Where(p => p.ProductId == product.ProductId);
        }
        public ProductPhoto GetPhoto(int id)
        {
            return db.ProductPhotos.ToArray().Where(p => p.PhotoId == id).First();
        }
        public void Create(ProductPhoto item)
        {
            db.ProductPhotos.Add(item);
        }
        public void Update(int id, ProductPhoto item)
        {
            ProductPhoto selectedPhoto = db.ProductPhotos.ToArray().Where(p => p.PhotoId == id).First();
            selectedPhoto = item;
            Save();
        }
        public void Delete(int id)
        {
            db.ProductPhotos.Remove(db.ProductPhotos.ToArray().Where(p => p.PhotoId == id).First());
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
