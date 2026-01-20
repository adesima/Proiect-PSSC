using Billing.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Data
{
    public class BillingContext : DbContext
    {
        public BillingContext(DbContextOptions<BillingContext> options) : base(options)
        {
        }

        public DbSet<InvoiceDto> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceDto>(entity =>
            {
                entity.ToTable("Invoice");
                entity.HasKey(i => i.InvoiceId);

                entity.Property(i => i.SubtotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
