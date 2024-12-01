using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entities;

namespace OrderApi.Infrastructure.Data
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)// Aquí falta el ': DbContext'
    {
        public DbSet<Order> Orders { get; set; }
    }
}
