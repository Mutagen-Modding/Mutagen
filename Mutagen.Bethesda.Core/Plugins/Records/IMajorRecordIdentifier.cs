namespace Mutagen.Bethesda.Plugins.Records;

public interface IMajorRecordIdentifierGetter : IFormKeyGetter
{
    /// <summary>
    /// The usually unique string identifier assigned to the Major Record
    /// </summary>
    string? EditorID { get; }
}

public record MajorRecordIdentifier : IMajorRecordIdentifierGetter
{
    public required FormKey FormKey { get; init; }
    public string? EditorID { get; init; }
    
    public virtual bool Equals(MajorRecordIdentifier? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FormKey.Equals(other.FormKey)
               && string.Equals(EditorID, other.EditorID, StringComparison.OrdinalIgnoreCase);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(FormKey, EditorID?.GetHashCode(StringComparison.OrdinalIgnoreCase));
    }
    
    public static IEqualityComparer<IMajorRecordIdentifierGetter> EqualityComparer => _equalityComparer;

    private static readonly Comparer _equalityComparer = new();

    class Comparer : IEqualityComparer<IMajorRecordIdentifierGetter>
    {
        public bool Equals(IMajorRecordIdentifierGetter? x, IMajorRecordIdentifierGetter? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.FormKey.Equals(y.FormKey)
                   && string.Equals(x.EditorID, y.EditorID, StringComparison.OrdinalIgnoreCase);
        }
        
        public int GetHashCode(IMajorRecordIdentifierGetter obj)
        {
            return HashCode.Combine(obj.FormKey, obj.EditorID?.GetHashCode(StringComparison.OrdinalIgnoreCase));
        }
    }
}

public interface IMajorRecordIdentifierGetter<TMajorGetter> : IMajorRecordIdentifierGetter
    where TMajorGetter : class, IMajorRecordQueryableGetter
{
}

public record MajorRecordIdentifier<TMajorGetter> : MajorRecordIdentifier, IMajorRecordIdentifierGetter<TMajorGetter>
    where TMajorGetter : class, IMajorRecordQueryableGetter
{
}