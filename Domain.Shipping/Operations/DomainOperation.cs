using System.Threading.Tasks;

namespace Domain.Shipping.Operations
{
    public abstract class DomainOperation<TInput, TContext, TOutput>
    {
        public abstract TOutput Transform(TInput input, TContext context);
        
        // Versiune asincrona, utila daca ai nevoie de baza de date
        public virtual Task<TOutput> TransformAsync(TInput input, TContext context)
        {
            return Task.FromResult(Transform(input, context));
        }
    }
}