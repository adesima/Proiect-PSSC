// using System.Data.Entity;
using Shipping.Data.Models;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Shipping.Data
{
    public class ShippingContext : DbContext
    {
        public ShippingContext(DbContextOptions<ShippingContext> options) : base(options)
        {
        }

        // Tabela principală cu livrări
        public DbSet<ShipmentDto> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShipmentDto>(entity =>
            {
                // Numele tabelului în SQL
                entity.ToTable("Shipment");
                
                // Cheia primară
                entity.HasKey(s => s.ShipmentId);

                // IMPORTANȚĂ CRITICĂ: 
                // AWB-ul este unic și căutăm după el, deci îi punem un Index
                entity.HasIndex(s => s.Awb).IsUnique();

                // Configurăm precizia pentru bani (Costul transportului)
                entity.Property(s => s.ShippingCost).HasColumnType("decimal(18,2)");
            });
        }
    }
}