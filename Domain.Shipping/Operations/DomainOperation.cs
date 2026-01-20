// using System.Threading.Tasks;
//
// namespace Domain.Shipping.Operations
// {
//     public abstract class DomainOperation<TInput, TContext, TOutput>
//     {
//         public abstract TOutput Transform(TInput input, TContext context);
//         
//         // Versiune asincrona, utila daca ai nevoie de baza de date
//         public virtual Task<TOutput> TransformAsync(TInput input, TContext context)
//         {
//             return Task.FromResult(Transform(input, context));
//         }
//     }
// }

// using System.Threading.Tasks;

// namespace Domain.Shipping.Operations
// {
//     // Clasă abstractă care definește contractul pentru orice operație din domeniu.
//     // TInput: Starea care intră (ex: UnvalidatedShipment)
//     // TOutput: Starea care iese (ex: IShipment - pentru că poate ieși Validated sau Invalid)
//     public abstract class DomainOperation<TInput, TOutput>
//     {
//         public abstract TOutput Transform(TInput input);
//     }
// }


namespace Domain.Shipping.Operations;

public abstract class DomainOperation<TEntity, TState, TResult>
    where TEntity : notnull
    where TState : class
{
    // Metoda abstractă care primește entitatea (ex: ValidatedShipment) 
    // și o stare externă opțională (ex: ConfigurarePreturi)
    public abstract TResult Transform(TEntity entity, TState? state);
}