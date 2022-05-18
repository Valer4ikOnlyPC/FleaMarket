using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IStorageService
    {
        Task<string> UploadToCloud(string imgPath);
        Task DeleteFromCloud(string imgPath);
    }
}
