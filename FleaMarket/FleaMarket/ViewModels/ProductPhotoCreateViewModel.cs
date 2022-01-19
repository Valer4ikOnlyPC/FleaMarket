using Microsoft.AspNetCore.Http;

namespace FleaMarket.ViewModels
{
    public class ProductPhotoCreateViewModel
    {
        public Guid PhotoId { get; set; }
        public IFormFile Photo { get; set; }
        public Guid ProductId { get; set; }
    }
}
