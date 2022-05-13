using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IFfmpegService
    {
        Task<string> ConvertImage(string imagePath);
        Task DeleteImages(string imagePath);
        Task<decimal[]> GetState(string imagePath);
    }
}
