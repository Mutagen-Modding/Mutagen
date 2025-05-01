namespace Mutagen.Bethesda.Plugins.Exceptions;

public class RecordCollisionException : Exception
{
    public readonly FormKey? FormKey;
    public readonly Type GroupType;
    public readonly ModKey ModKey;

    public RecordCollisionException(
        ModKey modKey,
        FormKey formKey,
        Type type)
        : base($"Two records with the same FormKey {formKey} existed in the same Group<{type.Name}> within the mod {modKey}")
    {
        ModKey = modKey;
        GroupType = type;
        FormKey = formKey;
    }
}