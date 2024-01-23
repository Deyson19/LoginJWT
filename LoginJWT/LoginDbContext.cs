using LoginJWT.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginJWT
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> op) : base(op)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Empleados> Empleados { get; set; }
    }
}