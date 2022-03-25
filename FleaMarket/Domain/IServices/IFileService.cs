using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IFileService
    {
        Task<IEnumerable<ProductPhoto>> UploadMany(IEnumerable<IFormFile> Image, Guid productId, int photoCount);
        Task<int> FileCheck(IFormFile formFile);
        Task DeletePhoto(string ImagePath);
    }
}
