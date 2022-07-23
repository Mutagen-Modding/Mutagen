namespace Mutagen.Bethesda.WPF.Reflection.Attributes;

[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false)]
public class FormLinkPickerCustomization : Attribute
{
    public Type[] ScopedTypes { get; }

    public FormLinkPickerCustomization(params Type[] scopedTypes)
    {
        ScopedTypes = scopedTypes;
    }
}