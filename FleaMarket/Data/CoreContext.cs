using Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CoreContext:DbContext
    {
        public CoreContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Category> Categorys { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-NHCFOI0\SQLEXPRESS;Initial Catalog=WebApp1;Integrated Security=True");
        }
    }
}
