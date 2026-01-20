using System.Threading.Tasks;
using Billing.Data.Models;
using Domain.Invoicing.Models;
using Domain.Invoicing.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Billing.Data.Repositories
{
    public class InvoicesRepository : IInvoicesRepository
    {
        private readonly BillingContext dbContext;

        public InvoicesRepository(BillingContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PaidInvoice?> GetByIdAsync(Guid id)
        {
            var entity = await dbContext.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (entity is null)
                return null;

            var currency = string.IsNullOrWhiteSpace(entity.Currency) ? "RON" : entity.Currency;

            return new PaidInvoice
            {
                InvoiceId = entity.InvoiceId,
                OrderId = entity.OrderId,
                CustomerId = entity.CustomerId,
                BillingAddress = BillingAddress.Create(
                    entity.County,
                    entity.City,
                    entity.Street,
                    entity.PostalCode),
                Lines = Array.Empty<OrderLine>(), 
                Subtotal = Money.Create(entity.SubtotalAmount, currency),
                Tax = Money.Create(entity.TaxAmount, currency),
                Total = Money.Create(entity.TotalAmount, currency),
                PaidAt = entity.PaidAt
            };
        }


        public async Task SaveAsync(IInvoicePaidEvent invoice)
        {
            var entity = await dbContext.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);

            if (entity is null)
            {
                entity = new InvoiceDto
                {
                    InvoiceId = invoice.InvoiceId
                };
                dbContext.Invoices.Add(entity);
            }

            entity.OrderId = invoice.OrderId;
            entity.CustomerId = invoice.CustomerId;

            entity.County = invoice.BillingAddress.County;
            entity.City = invoice.BillingAddress.City;
            entity.Street = invoice.BillingAddress.Street;
            entity.PostalCode = invoice.BillingAddress.PostalCode;

            entity.SubtotalAmount = invoice.Subtotal.Amount;
            entity.TaxAmount = invoice.Tax.Amount;
            entity.TotalAmount = invoice.Total.Amount;
            entity.Currency = invoice.Subtotal.Currency;

            entity.PaidAt = invoice.PaidAt;
            entity.IsPaid = true;

            await dbContext.SaveChangesAsync();
        }

    }
}
