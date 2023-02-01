using Microsoft.EntityFrameworkCore;
using Shop.Models.Store;

namespace Shop.Data
{
    public class StoreDbContext : DbContext
    {
        public DbSet<User> User { set; get; }
        public DbSet<Category> Category { set; get; }

        public StoreDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}