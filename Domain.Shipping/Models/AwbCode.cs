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
        
        Value = value;
    }

    public override string ToString() => Value;
    
    public static implicit operator string(AwbCode awb) => awb.Value;
}