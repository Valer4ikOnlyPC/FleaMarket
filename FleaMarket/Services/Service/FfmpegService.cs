using AspNetCore.Yandex.ObjectStorage.Models;
using AspNetCore.Yandex.ObjectStorage;
using AspNetCore.Yandex;
using Domain.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Configuration;

namespace Services.Service
{
    public class FfmpegService : IFfmpegService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FfmpegService> _logger;
        private readonly YandexStorageService _yandexStorageService;
        private string _path;
        private string _tempPath;
        private string _imagePath;
        private string _imagePathMin;

        public FfmpegService(ILogger<FfmpegService> logger, IConfiguration configuration, YandexStorageService yandexStorageService)
        {
            var directory = Directory.GetCurrentDirectory();
            directory = directory.Remove(directory.Length - 22);
            _configuration = configuration;
            _logger = logger;
            _path = Path.Combine(directory, "Lib", "Ffmpeg");
            _tempPath = _configuration["TempFileDirectory"];
            _imagePath = _configuration["FileDirectory"];
            _imagePathMin = _configuration["FileDirectoryMin"];
            _yandexStorageService = yandexStorageService;
        }
        public async Task<string> ConvertImage(string imagePath)
        {
            var task1 = SaveImage(imagePath, " -vf scale=200:-1 ", _imagePathMin);
            var task2 = SaveImage(imagePath, " ", _imagePath);
            Task.WaitAll(task1, task2);
            return await task2;
        }
        public async Task DeleteImages(string imagePath)
        {
            await Task.Run(() =>
            {
                File.Delete(imagePath);
                File.Delete(imagePath.Replace("img", "imgMin"));
            });
        }
        public async Task<decimal[]> GetState(string imagePath)
        {
            var info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = _path,
                FileName = Path.Combine(_path, "ffprobe.exe"),
                Arguments = string.Concat("  -v verbose -print_format csv -show_frames ", imagePath),
            };
            using var process = new Process { StartInfo = info };
            process.Start();
            var results = process.StandardOutput.ReadToEnd().Split('"');
            await process.WaitForExitAsync();
            if (results.Length < 9) return new decimal[2];
            var result = string.Concat(results[7], ",", results[9]).Replace(" ", "").Split(',', ':');
            var latitude = decimal.Parse(result[0]) + (decimal.Parse(result[2]) / 60) + (decimal.Parse(result[4]) / 36000000);
            var longitude = decimal.Parse(result[6]) + (decimal.Parse(result[8]) / 60) + (decimal.Parse(result[10]) / 36000000);
            return new decimal[2] { latitude, longitude };
        }
        private async Task<string> SaveImage(string imagePath, string parametrs, string path)
        {
            var fileName = string.Concat(Path.GetFileNameWithoutExtension(imagePath));
            var savePath = Path.Combine(path, string.Concat(fileName, ".jpg"));
            var info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _path,
                FileName = Path.Combine(_path, "ffmpeg.exe"),
                Arguments = string.Concat("-i ", imagePath, parametrs, savePath),
            };
            using var process = new Process { StartInfo = info };
            process.Start();
            await process.WaitForExitAsync();
            return savePath;
        }
    }
}
