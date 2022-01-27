using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface ICityRepository
    {
        IEnumerable<City> GetAll();
        Task<City> GetById(int id);
    }
}
