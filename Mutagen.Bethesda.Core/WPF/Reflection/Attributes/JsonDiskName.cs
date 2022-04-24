namespace Mutagen.Bethesda.WPF.Reflection.Attributes;

[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false)]
public class JsonDiskName : Attribute
{
    public string Name { get; }

    public JsonDiskName(string name)
    {
        Name = name;
    }
}