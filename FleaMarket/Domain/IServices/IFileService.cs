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
        Task<IEnumerable<ProductPhoto>> UploadMany(IEnumerable<IFormFile> Image, Guid productId);
        Task<int> FileCheck(IFormFile formFile);
    }
}
