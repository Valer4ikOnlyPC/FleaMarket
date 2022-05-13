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
        private readonly YandexStorageService _yandexStorageService;
        private readonly ILogger<FileService> _logger;
        private readonly long _fileSizeLimit;
        private readonly string _filePath;
        private readonly string _filePathMin;
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
            YandexStorageService yandexStorageService, ILogger<FileService> logger)
        {
            _ffmpegService = ffmpegService;
            _configuration = configuration;
            _yandexStorageService = yandexStorageService;
            _logger = logger;
            _fileSizeLimit = Int32.Parse(configuration["FileSizeLimit"]);
            _filePath = _configuration["FileDirectory"];
            _filePathMin = _configuration["FileDirectoryMin"];
            _tempFilePath = _configuration["TempFileDirectory"];
        }
        public async Task<IEnumerable<ProductPhoto>> UploadMany(IEnumerable<IFormFile> Image, Guid productId, int photoCount)
        {
            List<ProductPhoto> files = new List<ProductPhoto>();
            var result = Image.Where(i => i.Length < _fileSizeLimit & i.Length > 0);
            var tasks = new List<Task>();
            foreach (IFormFile img in result.Take(photoCount))
            {
                tasks.Add(Task.Run(() =>
                {
                    var check = FileCheck(new BinaryReader(img.OpenReadStream()).ReadBytes(8));
                    if (check == FileType.Unknown)
                        return;//continue;

                    var photoId = Guid.NewGuid();
                    var relativePath = string.Concat(productId.ToString(), "--", photoId, ".", check.ToString().ToLower());
                    string filePath = Path.Combine(_tempFilePath, relativePath);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    img.CopyTo(fileStream);
                    fileStream.Close();

                    var location = _ffmpegService.GetState(filePath).GetAwaiter().GetResult();
                    var imgPath = _ffmpegService.ConvertImage(filePath).GetAwaiter().GetResult();
                    var imgLink = string.Concat("https://flea-market-backet.website.yandexcloud.net", "/",
                        "img", "/", Path.GetFileName(imgPath));

                    using FileStream fileImg = new FileStream(imgPath, FileMode.Open),
                        fileImgMin = new FileStream(imgPath.Replace("img", "imgMin"), FileMode.Open);
                    var task1 = _yandexStorageService.PutObjectAsync(fileImg, Path.Combine("img", Path.GetFileName(imgPath)));
                    var task2 = _yandexStorageService.PutObjectAsync(fileImgMin, Path.Combine("imgMin", Path.GetFileName(imgPath)));
                    Task.WaitAll(task1, task2);
                    fileImg.Close();
                    fileImgMin.Close();

                    _ffmpegService.DeleteImages(filePath).Wait();
                    files.Add(new ProductPhoto { PhotoId = photoId, Link = imgLink, ProductId = productId, Latitude = location[0], Longitude = location[1] });
                    File.Delete(filePath);
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return files;
        }
        public async Task DeletePhoto(string ImagePath)
        {
            _logger.LogError(Path.GetFileName(ImagePath));
            var filePath = Path.Combine(_filePath, Path.GetFileName(ImagePath));
            var tempFilePath = Path.Combine(_filePathMin, Path.GetFileName(ImagePath));
            await _yandexStorageService.DeleteObjectAsync(string.Concat("img", "/", Path.GetFileName(ImagePath)));
            await _yandexStorageService.DeleteObjectAsync(string.Concat("imgMin", "/", Path.GetFileName(ImagePath)));
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
