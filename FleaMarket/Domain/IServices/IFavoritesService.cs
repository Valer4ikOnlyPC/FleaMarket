using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
    public interface IFavoritesService
    {
        Task<IEnumerable<Favorite>> GetAll();
        Task<IEnumerable<Favorite>> GetByUser(User user);
        Task<Favorite> GetById(Guid id);
        Task<Guid> Create(Favorite item);
        Task Delete(Guid id);
    }
}
