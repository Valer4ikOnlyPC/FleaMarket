using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Favorite
    {
        public Guid FavoriteId { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
}
