namespace Mutagen.Bethesda.WPF.Reflection.Attributes;

[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false)]
public class Tooltip : Attribute
{
    public string Text { get; }

    public Tooltip(string text)
    {
        Text = text;
    }
}