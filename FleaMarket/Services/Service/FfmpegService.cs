using Domain.DTO;
using Domain.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FfmpegService : IFfmpegService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FfmpegService> _logger;
        private string _path;
        private string _imagePath;
        private string _imagePathMin;
        private const decimal Seconds = 36000000;
        private const decimal Minutes = 60;

        public FfmpegService(ILogger<FfmpegService> logger, IConfiguration configuration)
        {
            var directory = Directory.GetCurrentDirectory();
            var rootPath = Path.GetDirectoryName(Path.GetDirectoryName(directory));
            _configuration = configuration;
            _logger = logger;
            _path = Path.Combine(rootPath, "Lib", "Ffmpeg");
            _imagePath = _configuration["FileDirectory"];
            _imagePathMin = _configuration["FileDirectoryMin"];
        }
        public async Task<string> ConvertImage(string imagePath)
        {
            var task1 = SaveImage(imagePath, " -vf scale=200:-1 ", _imagePathMin);
            var task2 = SaveImage(imagePath, " ", _imagePath);
            await Task.WhenAll(task1, task2);
            return await task2;
        }
        public async Task DeleteImages(string imagePath)
        {
            var task1 = Task.Run(() => File.Delete(imagePath));
            var task2 = Task.Run(() => File.Delete(imagePath.Replace("img", "imgMin")));
            await Task.WhenAll(task1, task2);
        }
        public async Task<ImageInfoDto> GetImageInfo(string imagePath)
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
            if (results.Length < 9) return new ImageInfoDto();
            var result = string.Concat(results[7], ",", results[9]).Replace(" ", "").Split(',', ':');
            var latitude = decimal.Parse(result[0]) + (decimal.Parse(result[2]) / Minutes) + (decimal.Parse(result[4]) / Seconds);
            var longitude = decimal.Parse(result[6]) + (decimal.Parse(result[8]) / Minutes) + (decimal.Parse(result[10]) / Seconds);
            return new ImageInfoDto() { Latitude = latitude, Longitude = longitude };
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
