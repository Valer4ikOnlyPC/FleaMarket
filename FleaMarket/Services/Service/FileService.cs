using AspNetCore.Yandex.ObjectStorage;
using Domain.IServices;
using Domain.Models;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IFfmpegService _ffmpegService;
        private readonly IStorageService _storageService;
        private readonly ILogger<FileService> _logger;
        private readonly long _fileSizeLimit;
        private readonly string _tempFilePath;
        private enum FileType
        {
            Unknown,
            Jpg,
            Png
        }
        private static readonly Dictionary<FileType, byte[]> _fileSignature =
            new()
            {
                { FileType.Jpg, new byte[] { 0xFF, 0xD8, 0xFF } },
                { FileType.Png, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
            };

        public FileService(IConfiguration configuration, IFfmpegService ffmpegService, 
            ILogger<FileService> logger, IStorageService storageService)
        {
            _ffmpegService = ffmpegService;
            _storageService = storageService;
            _configuration = configuration;
            _logger = logger;
            _fileSizeLimit = Int32.Parse(configuration["FileSizeLimit"]);
            _tempFilePath = _configuration["TempFileDirectory"];
        }
        public async Task<IEnumerable<ProductPhoto>> UploadMany(IEnumerable<IFormFile> Image, Guid productId, int photoCount)
        {
            List<ProductPhoto> files = new List<ProductPhoto>();
            var result = Image.Where(i => i.Length < _fileSizeLimit & i.Length > 0);
            var tasks = new List<Task>();
            foreach (IFormFile img in result.Take(photoCount))
            {
                tasks.Add(Task.Run(async () =>
                {
                    var check = FileCheck(new BinaryReader(img.OpenReadStream()).ReadBytes(8));
                    if (check == FileType.Unknown)
                        return;

                    var photoId = Guid.NewGuid();
                    var relativePath = string.Concat(productId.ToString(), "--", photoId, ".", check.ToString().ToLower());
                    string filePath = Path.Combine(_tempFilePath, relativePath);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    img.CopyTo(fileStream);
                    fileStream.Close();

                    var location = await _ffmpegService.GetImageInfo(filePath);
                    var imgPath = await _ffmpegService.ConvertImage(filePath);
                    var imgLink = await _storageService.UploadToCloud(imgPath);

                    await _ffmpegService.DeleteImages(filePath);
                    files.Add(new ProductPhoto { PhotoId = photoId, Link = imgLink, ProductId = productId, Latitude = location.Latitude, Longitude = location.Longitude });
                    File.Delete(filePath);
                }));
            }
            await Task.WhenAll(tasks);
            return files;
        }
        public async Task DeletePhoto(string ImagePath)
        {
            await _storageService.DeleteFromCloud(ImagePath);
        }
        public async Task<int> FileCheck(IFormFile formFile)
        {
            var check = FileCheck(new BinaryReader(formFile.OpenReadStream()).ReadBytes(8));
            return ((int)check);
        }
        private FileType FileCheck(ReadOnlyMemory<byte> bytes)
        {
            if (!MemoryMarshal.TryGetArray(bytes, out var memorySegment))
                return FileType.Unknown;
            foreach (var (fileType, signatures) in _fileSignature)
            {
                var headerBytes = memorySegment.Take(signatures.Length);
                var isSignatureFound = headerBytes.SequenceEqual(signatures);
                if (!isSignatureFound) continue;
                return fileType;
            }
            return FileType.Unknown;
        }
    }
}
