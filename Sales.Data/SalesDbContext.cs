using Microsoft.EntityFrameworkCore;
using Sales.Data.Models;

namespace Sales.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
        {
        }

        public DbSet<OrderDto> Orders { get; set; }
        public DbSet<OrderLineDto> OrderLines { get; set; }
        public DbSet<Product> Products { get; set; }
        
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          // Configurare Tabela Orders
          modelBuilder.Entity<Models.OrderDto>()
              .ToTable("Orders")
              .HasKey(o => o.OrderId);
      
          // Configurare Tabela OrderLines
          modelBuilder.Entity<Models.OrderLineDto>()
              .ToTable("OrderLines")
              .HasKey(ol => ol.OrderLineId);
      
          // Relația: O Comandă are multe Linii
          modelBuilder.Entity<Models.OrderLineDto>()
              .HasOne(ol => ol.Order)
              .WithMany(o => o.OrderLines)
              .HasForeignKey(ol => ol.OrderId)
              .OnDelete(DeleteBehavior.Cascade); 
      }
    }
}