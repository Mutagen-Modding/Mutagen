namespace Mutagen.Bethesda.WPF.Reflection.Attributes;

[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false)]
public class StaticEnumDictionary : Attribute
{
    public bool Enabled { get; }

    public StaticEnumDictionary(bool enable = true)
    {
        Enabled = true;
    }
}