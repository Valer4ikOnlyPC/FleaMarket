using AspNetCore.Yandex.ObjectStorage;
using Domain.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class StorageService: IStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly YandexStorageService _yandexStorageService;
        private readonly ILogger<StorageService> _logger;
        private readonly string _yandexCloudPath;
        public StorageService(ILogger<StorageService> logger, IConfiguration configuration, YandexStorageService yandexStorageService)
        {
            _logger = logger;
            _configuration = configuration;
            _yandexStorageService = yandexStorageService;
            _yandexCloudPath = _configuration["YandexCloud"];
        }
        public async Task<string> UploadToCloud(string imgPath)
        {
            using FileStream fileImg = new FileStream(imgPath, FileMode.Open),
                        fileImgMin = new FileStream(imgPath.Replace("img", "imgMin"), FileMode.Open);
            var task1 = _yandexStorageService.PutObjectAsync(fileImg, Path.Combine("img", Path.GetFileName(imgPath)));
            var task2 = _yandexStorageService.PutObjectAsync(fileImgMin, Path.Combine("imgMin", Path.GetFileName(imgPath)));
            await Task.WhenAll(task1, task2);
            fileImg.Close();
            fileImgMin.Close();
            return string.Concat(_yandexCloudPath, "/",
                        "img", "/", Path.GetFileName(imgPath));
        }
        public async Task DeleteFromCloud(string imgPath)
        {
            var task1 = _yandexStorageService.DeleteObjectAsync(string.Concat("img", "/", Path.GetFileName(imgPath)));
            var task2 = _yandexStorageService.DeleteObjectAsync(string.Concat("imgMin", "/", Path.GetFileName(imgPath)));
            await Task.WhenAll(task1, task2);
        }
    }
}
