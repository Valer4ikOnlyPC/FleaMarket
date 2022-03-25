using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IDialogRepository
    {
        Task<IEnumerable<Dialog>> GetAll();
        Task<Dialog> GetById(Guid id);
        Task<IEnumerable<Dialog>> GetByUser(Guid userId);
        Task<Guid> Create(Dialog item);
        Task Delete(Guid id);
        Task UpdateDate(Guid id);
        Task<Dialog> CheckSimilar(Dialog dialog);
    }
}
