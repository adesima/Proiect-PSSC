using System.Threading.Tasks;
using Domain.Invoicing.Models;

namespace Domain.Invoicing.Repositories
{
    public interface IInvoicesRepository
    {
        // Returns a paid invoice by its number, or null if not found
        Task<PaidInvoice?> GetByIdAsync(Guid invoiceId);

        // Saves a paid invoice to the repository
        Task SaveAsync(PaidInvoice invoice);
    }
}
