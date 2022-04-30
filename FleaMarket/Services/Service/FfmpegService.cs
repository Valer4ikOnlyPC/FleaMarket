using Domain.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FfmpegService : IFfmpegService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FfmpegService> _logger;
        private string _path;
        private string _tempPath;
        public FfmpegService(ILogger<FfmpegService> logger, IConfiguration configuration)
        {
            var directory = Directory.GetCurrentDirectory();
            directory = directory.Remove(directory.Length - 11);
            _configuration = configuration;
            _logger = logger;
            _path = Path.Combine(directory, "Services", "Ffmpeg");
            _tempPath = _configuration["TempFileDirectory"];
        }
        public async Task GetPath()
        {
            _logger.LogError(_path);
        }
        public async Task<string> ConvertImage(string imagePath)
        {
            var fileName = string.Concat(Path.GetFileName(imagePath));
            var savePath = Path.Combine(_tempPath, fileName);
            var info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = _path,
                FileName = Path.Combine(_path, "ffmpeg.exe"),
                Arguments = string.Concat("-i ", imagePath, " -an -vf scale=100x100 ", savePath),
            };
            using var process = new Process { StartInfo = info };
            process.Start();
            process.WaitForExit();
            return savePath;
        }
        public async Task GetState(string imagePath)
        {
            var info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                WorkingDirectory = _path,
                FileName = Path.Combine(_path, "ffprobe.exe"),
                Arguments = string.Concat(" -v quiet -print_format json -show_streams ", imagePath),
            };
            using var process = new Process { StartInfo = info };
            process.Start();
            var results = process.StandardOutput.ReadToEnd().Split(',');
            process.WaitForExit();
            _logger.LogInformation("Codec name - {results}", results[2]);
            _logger.LogInformation("Codec long name - {results}", results[3]);
            _logger.LogInformation("Codec type - {results}", results[5]);
        }

    }
}
