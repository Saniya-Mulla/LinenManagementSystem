using LinenManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LinenManagementSystem.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
        }

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<CartLog> CartLog => Set<CartLog>();

        public DbSet<Cart> Cart => Set<Cart>();

        public DbSet<CartLogDetail> CartLogDetail => Set<CartLogDetail>();

        public DbSet<Linen> Linens => Set<Linen>();
    }
}
