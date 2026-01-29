namespace ZincirApp.Extensions;

public record SelectionItem(string Value, string Code)
{
    public override string ToString()
    {
        return Value.ToString();
    }
}