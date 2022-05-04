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
            directory = directory.Remove(directory.Length - 22);
            _configuration = configuration;
            _logger = logger;
            _path = Path.Combine(directory, "Lib", "Ffmpeg");
            _tempPath = _configuration["TempFileDirectory"];
        }
        public async Task<string> ConvertImage(string imagePath)
        {
            var fileName = string.Concat(Path.GetFileName(imagePath));
            var savePath = Path.Combine(_tempPath, fileName);
            var info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _path,
                FileName = Path.Combine(_path, "ffmpeg.exe"),
                Arguments = string.Concat("-i ", imagePath, " -vf scale=200:-1 ", savePath),
            };
            using var process = new Process { StartInfo = info };
            process.Start();
            await process.WaitForExitAsync();
            return savePath;
        }
        public async Task<string> GetState(string imagePath)
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
            if (results.Length >= 9) return string.Concat(results[7], ";", results[9]).Replace(" ", "");
            return string.Empty;
        }

    }
}
