namespace Domain.Shipping.Models;

public record AwbCode
{
    public string Value { get; }

    public AwbCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("AWB code cannot be empty or null.");
        }
        
        // Aici poți pune reguli extra (ex: trebuie să înceapă cu "AWB-")
        // if (!value.StartsWith("AWB-")) throw ...

        Value = value;
    }

    // Ca să poți folosi obiectul direct în string-uri (ex: $"Codul este {awb}")
    public override string ToString() => Value;
    
    // Operator implicit ca să poți asigna un AwbCode direct într-un string dacă e nevoie
    public static implicit operator string(AwbCode awb) => awb.Value;
}