using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IFfmpegService
    {
        Task GetPath();
        Task<string> ConvertImage(string imagePath);
        Task GetState(string imagePath);
    }
}
