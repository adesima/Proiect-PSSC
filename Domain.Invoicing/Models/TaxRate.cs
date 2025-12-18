namespace Domain.Invoicing.Models;

public record TaxRate
{
    public decimal Percent { get; }

    private TaxRate(decimal percent)
    {
        Percent = percent;
    }

    public static TaxRate Create(decimal percent)
    {
        if (percent < 0 || percent > 1)
        {
            throw new ArgumentException("Tax rate must be between 0 and 1 (e.g. 0.19 for 19%).", nameof(percent));
        }

        return new TaxRate(percent);
    }

    public Money Apply(Money baseAmount)
    {
        var taxAmount = baseAmount.Amount * Percent;
        return Money.Create(taxAmount, baseAmount.Currency);
    }
}
