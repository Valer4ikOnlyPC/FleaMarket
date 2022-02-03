using Domain.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly long _fileSizeLimit;
        private static readonly Dictionary<string, List<byte[]>> _fileSignature =
            new Dictionary<string, List<byte[]>>
        {
            { ".jpeg & .png", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    new byte[] { 0x89, 0x50, 0x4E, 0x47 },
                }
            },
        };

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
            _fileSizeLimit = Int32.Parse(configuration["FileSizeLimit"]);
        }
        public async Task<IEnumerable<ProductPhoto>> UploadMany(IEnumerable<IFormFile> Image, Guid productId)
        {
            List<ProductPhoto> files = new List<ProductPhoto>();
            foreach (IFormFile img in Image.Take(5))
            {
                if (img.Length > 0 && img.Length < _fileSizeLimit)
                {

                    using (var reader = new BinaryReader(img.OpenReadStream()))
                    {
                        var signatures = _fileSignature.First().Value;
                        var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                        Console.WriteLine(BitConverter.ToString(headerBytes));

                        if (! signatures.Any(signature =>
                            headerBytes.Take(signature.Length).SequenceEqual(signature)))
                            continue;
                    }

                    Guid photoId = Guid.NewGuid();
                    string filePath = Path.Combine(_configuration["FileDirectory"] + "/" + productId.ToString() + "--" + photoId + ".jpg");
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(fileStream);
                        files.Add(new ProductPhoto { PhotoId = photoId, Link = "/img/" + productId.ToString() + "--" + photoId + ".jpg", ProductId = productId });
                    }
                }
            }
            return files;
        }
        
    }
}
